using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public sealed class PasswordHash
{
    private const int SaltSize = 16;
    private readonly byte[] _salt;
    private readonly string _hash;

    public PasswordHash(string password) {
        using (var rng = new RNGCryptoServiceProvider()) {
            rng.GetBytes(_salt = new byte[SaltSize]);
        }
        var config = GetConfig(password);
        using (var argon2 = new Argon2(config))
        using (var hash = argon2.Hash()) {
            _hash = config.EncodeString(hash.Buffer);
        }
    }

    public Argon2Config GetConfig(string password) {
        return new Argon2Config {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            Threads = Environment.ProcessorCount,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = _salt, // >= 8 bytes if not null
            // Secret = secret, // from somewhere
            // AssociatedData = associatedData, // from somewhere
        };
    }

    public PasswordHash(byte[] salt, string hash) {
        Array.Copy(salt, 0, _salt = new byte[SaltSize], 0, SaltSize);
        _hash = hash;
    }

    public byte[] Salt => (byte[])_salt.Clone();

    public byte[] Hash => (byte[])_hash.Clone();

    public bool Verify(string password) {
        var config = GetConfig(password);
        SecureArray<byte> hashB = null;
        try {
            if (config.DecodeString(_hash, out hashB) && hashB != null) {
                using (var argon2 = new Argon2(config))
                using (var hashA = argon2.Hash()) {
                    var test = config.EncodeString(hashA.Buffer);
                    return hashB.Buffer.SequenceEqual(hashA.Buffer);
                }
            }
        } finally {
            hashB?.Dispose();
        }
        return false;
    }
}
