using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace XauDataLayerRust
{
    public static class EncriptacionRustPlus
    {
        private const string DllName = "xau_crypto.dll";

        [DllImport(
            DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "xau_encrypt"
        )]
        private static extern IntPtr NativeEncrypt(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string plainText,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string passPhrase,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string saltValue,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string hashAlgorithm,
            uint passwordIterations,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string initVector,
            uint keySize
        );

        [DllImport(
            DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "xau_decrypt"
        )]
        private static extern IntPtr NativeDecrypt(
            [MarshalAs(UnmanagedType.LPUTF8Str)] string cipherText,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string passPhrase,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string saltValue,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string hashAlgorithm,
            uint passwordIterations,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string initVector,
            uint keySize
        );

        [DllImport(
            DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "xau_free_string"
        )]
        private static extern void NativeFreeString(
            IntPtr ptr
        );


        public static string Encrypt(
            string plainText,
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int passwordIterations,
            string initVector,
            int keySize)
        {
            IntPtr result = NativeEncrypt(
                plainText,
                passPhrase,
                saltValue,
                hashAlgorithm,
                checked((uint)passwordIterations),
                initVector,
                checked((uint)keySize)
            );

            if (result == IntPtr.Zero)
            {
                throw new CryptographicException(
                    "Error al encriptar mediante XauCrypto."
                );
            }

            try
            {
                return Marshal.PtrToStringUTF8(result)
                    ?? throw new CryptographicException(
                        "La DLL devolvió una cadena inválida."
                    );
            }
            finally
            {
                NativeFreeString(result);
            }
        }


        public static string Decrypt(
            string cipherText,
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int passwordIterations,
            string initVector,
            int keySize)
        {
            IntPtr result = NativeDecrypt(
                cipherText,
                passPhrase,
                saltValue,
                hashAlgorithm,
                checked((uint)passwordIterations),
                initVector,
                checked((uint)keySize)
            );

            if (result == IntPtr.Zero)
            {
                throw new CryptographicException(
                    "Error al desencriptar mediante XauCrypto."
                );
            }

            try
            {
                return Marshal.PtrToStringUTF8(result)
                    ?? throw new CryptographicException(
                        "La DLL devolvió una cadena inválida."
                    );
            }
            finally
            {
                NativeFreeString(result);
            }
        }
    }
}
