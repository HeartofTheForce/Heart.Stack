using System;
using System.IO;
using System.Security.Cryptography;
using Heart.Auth.Logic.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Heart.Auth.Logic.Jwt.AuthGenerators
{
    public class AsymmetricAuthGenerator : IAuthGenerator
    {
        private AuthSettings AuthSettings { get; set; }

        public AsymmetricAuthGenerator(IOptions<AuthSettings> options)
        {
            AuthSettings = options.Value;
        }

        public SigningCredentials GenerateSigningCredentials()
        {
            RSA privateRSA = RSAUtils.FromPrivateKey(AuthSettings.PrivateKey);

            var signingKey = new RsaSecurityKey(privateRSA);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

            return signingCredentials;
        }

        public SecurityKey GenerateValidationKey()
        {
            RSA publicRSA = RSAUtils.FromPublicKey(AuthSettings.PublicKey);
            var signingKey = new RsaSecurityKey(publicRSA);

            return signingKey;
        }

        public class RSAUtils
        {
            public static RSA FromPrivateKey(string key)
            {
                byte[] keyBytes = Convert.FromBase64String(key);
                return DecodeRSAPrivateKey(keyBytes);
            }

            //------- Parses binary ans.1 RSA private key; returns RSA ---
            static RSA DecodeRSAPrivateKey(byte[] privateKey)
            {
                byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

                // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
                using (MemoryStream mem = new MemoryStream(privateKey))
                using (BinaryReader binr = new BinaryReader(mem))    //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes != 0x0102) //version number
                        return null;
                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;

                    MODULUS = binr.ReadBytes(GetIntegerSize(binr));
                    E = binr.ReadBytes(GetIntegerSize(binr));
                    D = binr.ReadBytes(GetIntegerSize(binr));
                    P = binr.ReadBytes(GetIntegerSize(binr));
                    Q = binr.ReadBytes(GetIntegerSize(binr));
                    DP = binr.ReadBytes(GetIntegerSize(binr));
                    DQ = binr.ReadBytes(GetIntegerSize(binr));
                    IQ = binr.ReadBytes(GetIntegerSize(binr));

                    // ------- create RSA from private key RSAParameters -----
                    RSAParameters rsaParameters = new RSAParameters
                    {
                        Modulus = MODULUS,
                        Exponent = E,
                        D = D,
                        P = P,
                        Q = Q,
                        DP = DP,
                        DQ = DQ,
                        InverseQ = IQ,
                    };
                    return RSA.Create(rsaParameters);
                }
            }

            public static RSA FromPublicKey(string key)
            {
                byte[] keyBytes = Convert.FromBase64String(key);
                return DecodeX509PublicKey(keyBytes);
            }

            static RSA DecodeX509PublicKey(byte[] x509Key)
            {
                // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
                // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
                using (MemoryStream mem = new MemoryStream(x509Key))
                using (BinaryReader binr = new BinaryReader(mem))    //wrap Memory Stream with BinaryReader for easy reading
                {
                    ushort twobytes = binr.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            binr.ReadByte();    //advance 1 byte
                            break;
                        case 0x8230:
                            binr.ReadInt16();   //advance 2 bytes
                            break;
                        default:
                            return null;
                    }

                    byte[] seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, seqOid))  //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    byte bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    byte firstbyte = binr.ReadByte();
                    binr.BaseStream.Seek(-1, SeekOrigin.Current);

                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize); //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSA from public key RSAParameters -----
                    RSAParameters rsaParameters = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    return RSA.Create(rsaParameters);
                }
            }

            private static int GetIntegerSize(BinaryReader binr)
            {
                byte bt = 0;
                byte lowbyte = 0x00;
                byte highbyte = 0x00;
                int count = 0;
                bt = binr.ReadByte();
                if (bt != 0x02)     //expect integer
                    return 0;
                bt = binr.ReadByte();

                if (bt == 0x81)
                    count = binr.ReadByte();    // data size in next byte
                else
                    if (bt == 0x82)
                {
                    highbyte = binr.ReadByte(); // data size in next 2 bytes
                    lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;     // we already have the data size
                }

                while (binr.ReadByte() == 0x00)
                {   //remove high order zeros in data
                    count -= 1;
                }
                binr.BaseStream.Seek(-1, SeekOrigin.Current);     //last ReadByte wasn't a removed zero, so back up a byte
                return count;
            }

            static bool CompareBytearrays(byte[] a, byte[] b)
            {
                if (a.Length != b.Length)
                    return false;
                int i = 0;
                foreach (byte c in a)
                {
                    if (c != b[i])
                        return false;
                    i++;
                }
                return true;
            }
        }
    }
}
