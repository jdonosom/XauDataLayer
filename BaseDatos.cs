using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Security.Cryptography;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Text.Json;
using System.IO;
using XauCfg;


namespace XauDataLayer
{
    public class BaseDatos
    {
        //string passPhrase = "Pa55pr@se";  // can be any string
        //string saltValue = "s@1tV@lue";  // can be any string
        //string hashAlgorithm = "SHA1";       // can be "MD5"
        //int passwordIterations = 2;            // can be any number
        //string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
        //int keySize = 256; // can be 192 or 128

        #region Propiedades publicas
        public string Provider { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        #endregion

        private DbConnection conexion = null;
        private DbCommand comando = null;
        private DbTransaction transaccion = null;
        private string cadenaConexion;

        private static DbProviderFactory factory = null;

        /// <summary>
        /// Crea una instancia del acceso a la base de datos.
        /// </summary>
        public BaseDatos()
        {
            CargaSecretos();
            CargaParametros();
            Configurar();
        }

        public BaseDatos(string secret, string credencial)
        {
            // Verificar si existen las variable de entorno
            string jsonSecret = Environment.GetEnvironmentVariable("secrets");
            if (jsonSecret == null)
            {
                throw new BaseDatosException($"No se encontraron las variables de entorno (\"{secret}\").");
            }

            string jsonCredencial = Environment.GetEnvironmentVariable("credencial");
            if (jsonCredencial == null)
            {
                throw new BaseDatosException($"No se encontraron las variables de entorno (\"{credencial}\").");
            }

            // Deserializar las cadenas
            var config = JsonSerializer.Deserialize<Parametros>(jsonSecret);
            if (config == null)
            {
                throw new BaseDatosException($"Error en el archivo de configuración secret.");
            }
            var configCred = JsonSerializer.Deserialize<XauCfg.Config>(jsonCredencial);
            if (configCred == null)
            {
                throw new BaseDatosException($"Error en el archivo de configuración configaccess.");
            }

            // Cargar los datos
            CargaSecretos(config);
            CargaParametros(configCred);
            Configurar();

        }



        public BaseDatos(string servidor, string port, string userName, string password, string provider, string dataBase)
        {
            Server = servidor;
            Port = port;
            User = userName;
            Password = password;
            Database = dataBase;
            Provider = provider;

            Configurar();
        }

        void CargaParametros()
        {
            string json = File.ReadAllText("ConfigAccess.json", Encoding.UTF8);
            var config = JsonSerializer.Deserialize<XauCfg.Config>(json);

            // Desencriptar los campos
            if (config != null)
            {
                CargaParametros(config);

            }
        }
        private void CargaParametros(XauCfg.Config config)
        {
            // Desencriptar los campos
            if (config == null)
            {
                throw new BaseDatosException("No se pudo cargar la configuración de credenciales.");
            }
            Provider = config.Adonet.Encriptado ? EncriptacionPlus
                .Decrypt(config.Adonet.Proveedor
                , ParametrosClave.Frase
                , ParametrosClave.Salt
                , ParametrosClave.Algorit
                , ParametrosClave.Iteraciones
                , ParametrosClave.Vector
                , ParametrosClave.TamanoClave) : config.Adonet.Proveedor;

            Server = config.Server.Encriptado ? EncriptacionPlus
                .Decrypt(config.Server.Nombre
                , ParametrosClave.Frase
                , ParametrosClave.Salt
                , ParametrosClave.Algorit
                , ParametrosClave.Iteraciones
                , ParametrosClave.Vector
                , ParametrosClave.TamanoClave) : config.Server.Nombre;

            Port = config.Server.Encriptado ? EncriptacionPlus
                .Decrypt(config.Server.Puerto
                , ParametrosClave.Frase
                , ParametrosClave.Salt
                , ParametrosClave.Algorit
                , ParametrosClave.Iteraciones
                , ParametrosClave.Vector
                , ParametrosClave.TamanoClave) : config.Server.Puerto;

            User = config.User.Encriptado ? EncriptacionPlus
                .Decrypt(config.User.Nombre
                , ParametrosClave.Frase
                , ParametrosClave.Salt
                , ParametrosClave.Algorit
                , ParametrosClave.Iteraciones
                , ParametrosClave.Vector
                , ParametrosClave.TamanoClave) : config.User.Nombre;

            Password = config.Password.Encriptado ? EncriptacionPlus
                .Decrypt(config.Password.Secreto
                , ParametrosClave.Frase
                , ParametrosClave.Salt
                , ParametrosClave.Algorit
                , ParametrosClave.Iteraciones
                , ParametrosClave.Vector
                , ParametrosClave.TamanoClave) : config.Password.Secreto;

            Database = config.Database.Encriptado ? EncriptacionPlus
                .Decrypt(config.Database.Nombre
                , ParametrosClave.Frase
                , ParametrosClave.Salt
                , ParametrosClave.Algorit
                , ParametrosClave.Iteraciones
                , ParametrosClave.Vector
                , ParametrosClave.TamanoClave) : config.Database.Nombre;

        }
        private void CargaSecretos()
        {
            string json = null;
            // si existen los archivos de configuracion json, cargar los campos
            if (File.Exists("secrets.json"))
            {
                json = File.ReadAllText("secrets.json", Encoding.UTF8);

            }

            if (json == null)
            {
                throw new BaseDatosException("No se encontraron los archivos de configuración.");
            }

            var config = JsonSerializer.Deserialize<Parametros>(json);
            CargaSecretos(config);


        }

        private void CargaSecretos(Parametros config)
        {
            if (config == null)
            {
                throw new BaseDatosException("No se pudo cargar los secretos de configuración.");
            }

            ParametrosClave.Frase = config.Frase;
            ParametrosClave.Salt = config.Salt;
            ParametrosClave.Algorit = config.Algorit;
            ParametrosClave.Iteraciones = config.Iteraciones;
            ParametrosClave.Vector = config.Vector;
            ParametrosClave.TamanoClave = config.TamanoClave;
        }


        /// <summary>
        /// Configura el acceso a la base de datos para su utilización.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al cargar la configuración.</exception>
        private void Configurar()
        {
            // Valida que las propiedades qeu la funcion utilizar esten seteados
            if (string.IsNullOrEmpty(Server)
                || string.IsNullOrEmpty(Port)
                || string.IsNullOrEmpty(Database)
                || string.IsNullOrEmpty(User)
                || string.IsNullOrEmpty(Password)
                || string.IsNullOrEmpty(Provider))
            {
                throw new BaseDatosException("Error en parámetros de la cadena de conexión.");
            }

            try
            {
                this.cadenaConexion = String.Format($"Server={Server};Database={Database};Uid={User};Pwd={Password}");

                switch (Provider)
                {
                    case "System.Data.SqlClient":
                        DbProviderFactories.RegisterFactory(Provider, System.Data.SqlClient.SqlClientFactory.Instance);
                        break;
                    case "MySql.Data.MySqlClient":
                        DbProviderFactories.RegisterFactory(Provider, MySql.Data.MySqlClient.MySqlClientFactory.Instance);
                        break;
                    default:
                        break;
                }
                BaseDatos.factory = DbProviderFactories.GetFactory(Provider);

                //if (Provider.Equals("System.Data.SqlClient"))
                //    BaseDatos.factory = DbProviderFactories.GetFactory(Provider);
            }
            catch (ConfigurationException ex)
            {
                throw new BaseDatosException("Error al cargar la configuración del acceso a datos.", ex);
            }
        }

        private static DbProviderFactory GetFactory()
        {
            // register SqlClientFactory in provider factories
            //DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

            return DbProviderFactories.GetFactory("Microsoft.Data.SqlClient");
        }


        /// <summary>
        /// Permite desconectarse de la base de datos.
        /// </summary>
        public void Desconectar()
        {
            if (this.conexion.State.Equals(ConnectionState.Open))
            {
                this.conexion.Close();
                this.conexion = null;
                //this.conexion.Dispose();
            }
        }

        /// <summary>
        /// Se concecta con la base de datos.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al conectarse.</exception>
        public void Conectar()
        {
            if (this.conexion != null && !this.conexion.State.Equals(ConnectionState.Closed))
            {
                return;
                // throw new BaseDatosException("La conexión ya se encuentra abierta.");
            }
            try
            {
                if (this.conexion == null)
                {
                    this.conexion = factory.CreateConnection();
                    this.conexion.ConnectionString = cadenaConexion;
                }
                this.conexion.Open();
            }
            catch (DataException ex)
            {
                throw new BaseDatosException("Error al conectarse a la base de datos.", ex);
            }
            catch (Exception ex)
            {
                throw new BaseDatosException("Error al conectarse a la base de datos.", ex);
            }
        }

        /// <summary>
        /// Crea un comando en base a una sentencia SQL.
        /// Ejemplo:
        /// <code>SELECT * FROM Tabla WHERE campo1=@campo1, campo2=@campo2</code>
        /// Guarda el comando para el seteo de parámetros y la posterior ejecución.
        /// </summary>
        /// <param name="sentenciaSQL">La sentencia SQL con el formato: SENTENCIA [param = @param,]</param>
        public void CrearComando(string sentenciaSQL)
        {
            this.comando = factory.CreateCommand();
            this.comando.Connection = this.conexion;
            this.comando.CommandType = CommandType.Text;
            this.comando.CommandText = sentenciaSQL;
            if (this.transaccion != null)
            {
                this.comando.Transaction = this.transaccion;
            }
        }

        /// <summary>
        /// Setea un parámetro como nulo del comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro cuyo valor será nulo.</param>
        public void AsignarParametroNulo(string nombre)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Object;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = DBNull.Value;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "", "NULL");
        }

        /// <summary>
        /// Asigna un parámetro de tipo cadena al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroCadena(string nombre, string valor)
        {
            DbParameter param = comando.CreateParameter();
            param.DbType = System.Data.DbType.String;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Size = valor.Length;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "'", valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo Boolean al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroBoolean(string nombre, Boolean valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Boolean;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "", valor.ToString());
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroEntero(string nombre, int valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Int32;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "", valor.ToString());
        }

        /// <summary>
        /// Asigna un parámetro de tipo double al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroDouble(string nombre, double valor)
        {

            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Double;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "", valor.ToString("#.#"));
        }

        /// <summary>
        /// Asigna un parámetro de tipo double al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroDecimal(string nombre, decimal valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Decimal;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);
        }

        /// <summary>
        /// Asigna un parámetro de tipo double al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroFloat(string nombre, float valor)
        {

            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Double;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "", valor.ToString("#.#"));
        }


        //public void AsignarParametroImage(string nombre, System.IO.MemoryStream valor = null )
        public void AsignarParametroImage(string nombre, byte[] valor = null)
        {
            DbParameter param = comando.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            // param.Value = valor.GetBuffer();
            param.Value = valor;

            comando.Parameters.Add(param);

            // this.comando.Parameters.Add(SqlDbType.Image);
            //this.comando.Parameters[nombre].Value = valor.GetBuffer();
        }

        /// <summary>
        /// Asigna un parámetro al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="separador">El separador que será agregado al valor del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        private void AsignarParametro(string nombre, string separador, string valor)
        {
            int indice = this.comando.CommandText.IndexOf(nombre);
            string prefijo = this.comando.CommandText.Substring(0, indice);
            string sufijo = this.comando.CommandText.Substring(indice + nombre.Length);
            this.comando.CommandText = prefijo + separador + valor + separador + sufijo;
        }

        /// <summary>
        /// Asigna un parámetro de tipo fecha al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroFecha(string nombre, DateTime? valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.DateTime;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "'", valor.ToString());
        }

        /// <summary>
        /// Ejecuta el comando creado y retorna el resultado de la consulta.
        /// </summary>
        /// <returns>El resultado de la consulta.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el comando.</exception>
        public DbDataReader EjecutarConsulta()
        {
            return this.comando.ExecuteReader();
        }

        /// <summary>
        /// Ejecuta el comando creado y retorna un escalar.
        /// </summary>
        /// <returns>El escalar que es el resultado del comando.</returns>
        /// <exception cref="BaseDatosException">Si ocurre un error al ejecutar el comando.</exception>
        public float EjecutarEscalar()
        {
            float escalar = 0;
            try
            {
                var result = this.comando.ExecuteScalar();

                // this.comando.CommandType = CommandType.StoredProcedure;
                if (result != null)
                {
                    escalar = float.Parse(result.ToString());
                }
            }
            catch (InvalidCastException ex)
            {
                throw new BaseDatosException("Error al ejecutar un escalar.", ex);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {

            }
            return escalar;
        }

        /// <summary>
        /// Ejecuta el comando creado.
        /// </summary>
        public void EjecutarComando()
        {
            this.comando.ExecuteNonQuery();
        }

        /// <summary>
        /// Comienza una transacción en base a la conexion abierta.
        /// Todo lo que se ejecute luego de esta ionvocación estará 
        /// dentro de una tranasacción.
        /// </summary>
        public void ComenzarTransaccion()
        {
            if (this.transaccion == null)
            {
                this.transaccion = this.conexion.BeginTransaction();
            }
        }

        /// <summary>
        /// Cancela la ejecución de una transacción.
        /// Todo lo ejecutado entre ésta invocación y su 
        /// correspondiente <c>ComenzarTransaccion</c> será perdido.
        /// </summary>
        public void CancelarTransaccion()
        {
            if (this.transaccion != null)
            {
                this.transaccion.Rollback();
            }
        }

        /// <summary>
        /// Confirma todo los comandos ejecutados entre el <c>ComanzarTransaccion</c>
        /// y ésta invocación.
        /// </summary>
        public void ConfirmarTransaccion()
        {
            if (this.transaccion != null)
            {
                this.transaccion.Commit();
            }
        }

        public string EncryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);

            // RSACryptoServiceProvider here;

            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[
                        (dataLength - maxLength * i > maxLength) ? maxLength :
                                                      dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0,
                                  tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes,
                                                                          true);
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        public string DecryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(inputString.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }


    }
}
