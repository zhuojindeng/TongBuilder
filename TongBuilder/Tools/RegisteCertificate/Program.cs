// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

Console.WriteLine("Hello, World!");

using var algorithm = RSA.Create(keySizeInBits: 2048);

 var subject = new X500DistinguishedName("CN=TongBuilder Encryption Certificate");
var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));

var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

File.WriteAllBytes("identity-encryption.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));

using var algorithm1 = RSA.Create(keySizeInBits: 2048);

var subject1 = new X500DistinguishedName("CN=TongBuilder Signing Certificate");
var request1 = new CertificateRequest(subject1, algorithm1, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
request1.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

var certificate1 = request1.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

File.WriteAllBytes("identity-signing.pfx", certificate1.Export(X509ContentType.Pfx, string.Empty));