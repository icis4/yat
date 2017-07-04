openssl genrsa -out cert.key 2048

openssl req -new -subj /C=BR/ST=TESTCERT/L=TESTCERT/O=TESTCERT/OU=TESTCERT/emailAddress=TESTCERT -key cert.key -out cert.csr

openssl x509 -req -sha256 -days 365 -in cert.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out cert.crt

pause