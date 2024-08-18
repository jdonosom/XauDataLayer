using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace XauDataLayer
{

    public class EncriptacionPlus
    {
        /// <summary>
        /// Encripta el texto plano especificado usando el algoritmo de clave simétrica AES
        /// y devuelve un resultado codificado en base64.
        /// </summary>
        /// <param name="plainText">Texto plano a ser encriptado.</param>
        /// <param name="passPhrase">Frase de paso usada para generar la clave de encriptación.</param>
        /// <param name="saltValue">Valor de sal usado junto con la frase de paso para generar la clave.</param>
        /// <param name="hashAlgorithm">Algoritmo hash utilizado para generar la clave. Valores permitidos: "MD5" y "SHA1".</param>
        /// <param name="passwordIterations">Número de iteraciones usadas para generar la clave.</param>
        /// <param name="initVector">Vector de inicialización (IV). Debe tener exactamente 16 caracteres ASCII.</param>
        /// <param name="keySize">Tamaño de la clave de encriptación en bits. Valores permitidos: 128, 192 y 256.</param>
        /// <returns>Valor encriptado en formato base64.</returns>
        public static string Encrypt(
            string plainText,
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int passwordIterations,
            string initVector,
            int keySize)
        {
            try
            {
                // Convertir el vector de inicialización y el valor de sal en arreglos de bytes.
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                // Convertir el texto plano en un arreglo de bytes.
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                // Crear una clave a partir de la frase de paso y la sal usando Rfc2898DeriveBytes.
                using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations, new HashAlgorithmName(hashAlgorithm)))
                {
                    // Obtener los bytes de la clave a partir del tamaño de la clave especificado.
                    byte[] keyBytes = password.GetBytes(keySize / 8);

                    // Inicializar el objeto de encriptación AES.
                    using (Aes aesAlg = Aes.Create())
                    {
                        // Configurar el modo de encriptación a CBC.
                        aesAlg.Mode = CipherMode.CBC;
                        aesAlg.Key = keyBytes;
                        aesAlg.IV = initVectorBytes;

                        // Crear el encriptador a partir de los bytes de la clave y el vector de inicialización.
                        using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                        // Definir un flujo de memoria para almacenar los datos encriptados.
                        using (MemoryStream memoryStream = new MemoryStream())
                        // Crear un flujo criptográfico para realizar la encriptación.
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            // Escribir los bytes del texto plano en el flujo criptográfico.
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            // Finalizar la encriptación.
                            cryptoStream.FlushFinalBlock();
                            // Obtener los bytes del texto encriptado desde el flujo de memoria.
                            byte[] cipherTextBytes = memoryStream.ToArray();
                            // Devolver el texto encriptado en formato base64.
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                // Manejar excepciones de argumentos nulos.
                Console.Error.WriteLine("Error: Uno de los parámetros de entrada es nulo. " + ex.Message);
                throw;
            }
            catch (EncoderFallbackException ex)
            {
                // Manejar errores de codificación.
                Console.Error.WriteLine("Error: Se produjo un error de codificación. " + ex.Message);
                throw;
            }
            catch (CryptographicException ex)
            {
                // Manejar errores criptográficos.
                Console.Error.WriteLine("Error: Se produjo un error criptográfico. " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Manejar todas las demás excepciones.
                Console.Error.WriteLine("Se produjo un error inesperado. " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Desencripta el texto cifrado especificado usando el algoritmo de clave simétrica AES.
        /// </summary>
        /// <param name="cipherText">Texto cifrado en formato base64.</param>
        /// <param name="passPhrase">Frase de paso usada para generar la clave de desencriptación.</param>
        /// <param name="saltValue">Valor de sal usado junto con la frase de paso para generar la clave.</param>
        /// <param name="hashAlgorithm">Algoritmo hash utilizado para generar la clave. Valores permitidos: "MD5" y "SHA1".</param>
        /// <param name="passwordIterations">Número de iteraciones usadas para generar la clave.</param>
        /// <param name="initVector">Vector de inicialización (IV). Debe tener exactamente 16 caracteres ASCII.</param>
        /// <param name="keySize">Tamaño de la clave de desencriptación en bits. Valores permitidos: 128, 192 y 256.</param>
        /// <returns>Texto desencriptado.</returns>
        public static string Decrypt(
            string cipherText,
            string passPhrase,
            string saltValue,
            string hashAlgorithm,
            int passwordIterations,
            string initVector,
            int keySize)
        {
            try
            {
                // Convertir el vector de inicialización y el valor de sal en arreglos de bytes.
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

                // Convertir el texto cifrado en un arreglo de bytes.
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                // Crear una clave a partir de la frase de paso y la sal usando Rfc2898DeriveBytes.
                using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations, new HashAlgorithmName(hashAlgorithm)))
                {
                    // Obtener los bytes de la clave a partir del tamaño de la clave especificado.
                    byte[] keyBytes = password.GetBytes(keySize / 8);

                    // Inicializar el objeto de desencriptación AES.
                    using (Aes aesAlg = Aes.Create())
                    {
                        // Configurar el modo de encriptación a CBC.
                        aesAlg.Mode = CipherMode.CBC;
                        aesAlg.Key = keyBytes;
                        aesAlg.IV = initVectorBytes;

                        // Crear el desencriptador a partir de los bytes de la clave y el vector de inicialización.
                        using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                        // Definir un flujo de memoria para almacenar los datos desencriptados.
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        // Crear un flujo criptográfico para realizar la desencriptación.
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            // Crear un buffer para almacenar los datos desencriptados.
                            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                            // Leer los bytes del flujo criptográfico.
                            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            // Devolver el texto desencriptado.
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                // Manejar excepciones de argumentos nulos.
                Console.Error.WriteLine("Error: Uno de los parámetros de entrada es nulo. " + ex.Message);
                throw;
            }
            catch (FormatException ex)
            {
                // Manejar errores de formato (por ejemplo, cadena base64 no válida).
                Console.Error.WriteLine("Error: Se produjo un error de formato. " + ex.Message);
                throw;
            }
            catch (CryptographicException ex)
            {
                // Manejar errores criptográficos.
                Console.Error.WriteLine("Error: Se produjo un error criptográfico. " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Manejar todas las demás excepciones.
                Console.Error.WriteLine("Se produjo un error inesperado. " + ex.Message);
                throw;
            }
        }
    }

}
