openssl genrsa -out ca.key 2048

openssl req -new -x509 -days 365 -sha256 -subj /C=BR/ST=TESTCA/L=TESTCA/O=TESTCA/OU=TESTCA/emailAddress=TESTCA -key ca.key -out ca.crt

pause