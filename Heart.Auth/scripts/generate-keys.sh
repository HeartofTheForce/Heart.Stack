#!/bin/bash
private=$(openssl genrsa 2048 2>/dev/null)
public=$(echo "${private[*]}" | openssl rsa -pubout 2>/dev/null)
echo "${private[*]}" | tr -d '\r'| tr -d '\n' | sed 's/\(-----BEGIN RSA PRIVATE KEY-----\|-----END RSA PRIVATE KEY-----\)//g' > private.key
echo "${public[*]}" | tr -d '\r'| tr -d '\n' | sed 's/\(-----BEGIN PUBLIC KEY-----\|-----END PUBLIC KEY-----\)//g' > public.key