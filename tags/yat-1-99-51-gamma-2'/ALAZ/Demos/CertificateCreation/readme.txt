Both CA and server certificate was created using Win32 OpenSSL (http://www.slproweb.com/products/Win32OpenSSL.html).
Download Win32 OpenSSL, install it and execute the following batch files in Win32 OpenSSL bin folder (openssl.exe location):

1) createCA.bat				- Create CA certificate
2) createCertandSign.bat	- Create server certificate and sign it using CA certificate
3) createPFX.bat			- Create PFX (P12) encrypted certificate to use with Windows

Files created:

ca.key / ca.crt	- CA key pair and certificate.
cert.csr		- server certificate request.

cert.crt		- server certificate (public key only)

	Only this file is deployed on client when using EncryptType.etRijndael. Load the file on OnSymmetricAuthenticate.
	If EncryptType.etSSL is used instead, there's no need to deploy this file in client.
	See EchoCryptService project for details.

cert.p12 		- server certificate (public/private key encrypted with password)

	This file will be always on server when using EncryptType.etRijndael or EncryptType.etSSL.
	If EncryptType.etSSL is used, load the file on OnSSLServerAuthenticate. If EncryptType.etRijndael is used, load the file on OnSymmetricAuthenticate.
	See EchoCryptService project for details.
