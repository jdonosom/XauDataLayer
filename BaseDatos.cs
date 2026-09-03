using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
            string jsonSecret = Environment.GetEnvironmentVariable(secret);
            if (jsonSecret == null)
            {
                throw new BaseDatosException($"No se encontraron las variables de entorno (\"{secret}\").");
            }

            string jsonCredencial = Environment.GetEnvironmentVariable(credencial);
            if (jsonCredencial == null)
            {
                throw new BaseDatosException($"No se encontraron las variables de entorno (\"{credencial}\").");
            }

            // Deserializar las cadenas
            var config = JsonSerializer.Deserialize<Secret>(jsonSecret);
            if (config == null)
            {
                throw new BaseDatosException($"Error en el archivo de configuración secret.");
            }


            var configCred = JsonSerializer.Deserialize<ConfigAccess>(jsonCredencial);
            if (configCred == null)
            {
                throw new BaseDatosException($"Error en el archivo de configuración configaccess.");
            }

            // Cargar los datos
            CargaSecretos(config);
            CargaParametros(configCred);
            Configurar();

        }



        public BaseDatos(string server, string port, string username, string password, string provider, string database)
        {
            Server = server;
            Port = port;
            User = username;
            Password = password;
            Database = database;
            Provider = provider;

            Configurar();
        }

        void CargaParametros()
        {
            string json = File.ReadAllText("ConfigAccess.json", Encoding.UTF8);
            var config = JsonSerializer.Deserialize<XauCfg.ConfigAccess>(json);

            // Desencriptar los campos
            if (config != null)
            {
                CargaParametros(config);

            }
        }
        private void CargaParametros(XauCfg.ConfigAccess config)
        {
            // Desencriptar los campos
            if (config == null)
            {
                throw new BaseDatosException("No se pudo cargar la configuración de credenciales.");
            }
            Provider = config.Adonet.Encriptado 
                ? EncriptacionPlus
                    .Decrypt(config.Adonet.Proveedor
                    , ParametrosClave.Frase
                    , ParametrosClave.Salt
                    , ParametrosClave.Algorit
                    , ParametrosClave.Iteraciones
                    , ParametrosClave.Vector
                    , ParametrosClave.TamanoClave) 
                : config.Adonet.Proveedor;

            Server = config.Server.Encriptado 
                ? EncriptacionPlus.Decrypt(config.Server.ServerName
                    , ParametrosClave.Frase
                    , ParametrosClave.Salt
                    , ParametrosClave.Algorit
                    , ParametrosClave.Iteraciones
                    , ParametrosClave.Vector
                    , ParametrosClave.TamanoClave) 
                : config.Server.ServerName;

            Port = config.Server.Encriptado 
                ? EncriptacionPlus
                    .Decrypt(config.Server.Port
                    , ParametrosClave.Frase
                    , ParametrosClave.Salt
                    , ParametrosClave.Algorit
                    , ParametrosClave.Iteraciones
                    , ParametrosClave.Vector
                    , ParametrosClave.TamanoClave) 
               : config.Server.Port;

            User = config.User.Encriptado 
                ? EncriptacionPlus
                    .Decrypt(config.User.UserName
                    , ParametrosClave.Frase
                    , ParametrosClave.Salt
                    , ParametrosClave.Algorit
                    , ParametrosClave.Iteraciones
                    , ParametrosClave.Vector
                    , ParametrosClave.TamanoClave) 
                : config.User.UserName;

            Password = config.Password.Encriptado 
                ? EncriptacionPlus
                    .Decrypt(config.Password.Secreto
                    , ParametrosClave.Frase
                    , ParametrosClave.Salt
                    , ParametrosClave.Algorit
                    , ParametrosClave.Iteraciones
                    , ParametrosClave.Vector
                    , ParametrosClave.TamanoClave) 
                : config.Password.Secreto;

            Database = config.Database.Encriptado 
                ? EncriptacionPlus
                    .Decrypt(config.Database.DatabaseName
                        , ParametrosClave.Frase
                        , ParametrosClave.Salt
                        , ParametrosClave.Algorit
                        , ParametrosClave.Iteraciones
                        , ParametrosClave.Vector
                        , ParametrosClave.TamanoClave) 
                : config.Database.DatabaseName;

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
            var config = JsonSerializer.Deserialize<Secret>(json);
            CargaSecretos(config);
        }

        private void CargaSecretos(Secret config)
        {
            if (config == null)
            {
                throw new BaseDatosException("No se pudo cargar los secretos de configuración.");
            }
            config.Guardar();
        }


        /// <summary>
        /// Configura el acceso a la base de datos para su utilización.
        /// </summary>
        /// <exception cref="BaseDatosException">Si existe un error al cargar la configuración.</exception>
        private void Configurar()
        {
            string ConnectionString = string.Empty;
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
                // Xport = $"{(Port.Equals("0") ? "1433" : $"{Port}")}";
                // 
                // sMySql = $"Server={Server};Port={Xport};Database={Database};Uid={User};Pwd={Password};"
                // sSqlSr = $"Server={Server},{Port};Database={Database};Uid={User};Pwd={Password};"
                //
                //string Xport = null;
                switch (Provider)
                {
                    case "System.Data.SqlClient":

                        ConnectionString = $"Server={Server},{Port};Database={Database};User Id={User};Password={Password};";

                        DbProviderFactories.RegisterFactory(Provider, System.Data.SqlClient.SqlClientFactory.Instance);
                        break;
                    case "MySql.Data.MySqlClient":
                        ConnectionString = Server + $"{(Port.Equals("0") ? ";Port=3306" : $";Port={Port}")}";
                        DbProviderFactories.RegisterFactory(Provider, MySql.Data.MySqlClient.MySqlClientFactory.Instance);
                        break;
                    case "Npgsql":
                        // Host=172.16.1.6;Port=5432;Database=documentgo;Username=jdonoso;Password=View1210;Pooling=true;Maximum Pool Size=50
                        //ConnectionString = $"User ID={User};Password={Password};Host={Server};Port={Port};Database={Database};Pooling=true;Connection Lifetime=0;";
                        ConnectionString = $"Host={Server};Port={Port};Database={Database};Username={User};Password={Password}";
                        DbProviderFactories.RegisterFactory(Provider, NpgsqlFactory.Instance);
                        break;
                    default:
                        break;
                }
                BaseDatos.factory = DbProviderFactories.GetFactory(Provider);
                this.cadenaConexion = ConnectionString;
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
        public void AsignarParametroCadena(string nombre, string? valor)
        {
            DbParameter param = comando.CreateParameter();
            param.DbType = System.Data.DbType.String;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Size = valor?.Length ?? 0;
            param.Value = valor is null ? DBNull.Value : valor;
            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "'", valor);
        }

        /// <summary>
        /// Asigna un parámetro de tipo Boolean al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroBoolean(string nombre, Boolean? valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Boolean;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor is null ? DBNull.Value : valor;
            comando.Parameters.Add(param);
        }

        /// <summary>
        /// Asigna un parámetro de tipo entero al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroEntero(string nombre, int? valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Int32;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor is null ? DBNull.Value : valor;
            comando.Parameters.Add(param);
        }

        /// <summary>
        /// Asigna un parámetro de tipo double al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroDouble(string nombre, double? valor)
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
        /// Asigna un parámetro de tipo long al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroLong(string nombre, long? valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Int64;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor is null ? DBNull.Value : valor;
            comando.Parameters.Add(param);
        }

        /// <summary>
        /// Asigna un parámetro de tipo double al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroDecimal(string nombre, decimal? valor)
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
        public void AsignarParametroFloat(string nombre, float? valor)
        {

            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.Double;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);

            // AsignarParametro(nombre, "", valor.ToString("#.#"));
        }


        public void AsignarParametroImage(string nombre, byte[]? valor)
        {
            DbParameter param = comando.CreateParameter();
            param.DbType = System.Data.DbType.Binary;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            // param.Value = valor.GetBuffer();
            param.Value = valor;

            comando.Parameters.Add(param);
        }

        public void AsignarParametroBytes(string nombre, byte[]? valor)
        {
            this.AsignarParametroImage(nombre, valor);
        }

        public void AsignarParametroByte(string nombre, byte? valor)
        {
            DbParameter param = comando.CreateParameter();
            param.DbType = System.Data.DbType.Byte;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;

            comando.Parameters.Add(param);
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
        /// Asigna un parámetro de tipo fecha al comando creado.
        /// </summary>
        /// <param name="nombre">El nombre del parámetro.</param>
        /// <param name="valor">El valor del parámetro.</param>
        public void AsignarParametroFechaOffSet(string nombre, DateTimeOffset? valor)
        {
            DbParameter param = comando.CreateParameter(); ;
            param.DbType = System.Data.DbType.DateTime;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = nombre;
            param.Value = valor;
            comando.Parameters.Add(param);
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
        /// Ejecuta una consulta y devuelve los resultados como una Lista de objetos de tipo T.
        /// Cubre los casos de 0, 1 o múltiples filas.
        /// </summary>
        public async Task<List<T>> EjecutarConsultaAsync<T>(
            string sentenciaSQL
            , object parametros = null) where T : new()
        {
            // 1. Asegurar conexión
            if (this.conexion == null || this.conexion.State != ConnectionState.Open)
            {
                this.Conectar();
            }

            // 2. Crear comando independiente (no interfiere con this.comando)
            using var cmd = factory.CreateCommand();
            cmd.Connection = this.conexion;
            cmd.CommandText = sentenciaSQL;
            cmd.CommandType = CommandType.Text;

            if (this.transaccion != null)
            {
                cmd.Transaction = this.transaccion;
            }

            // 3. Mapear parámetros desde el objeto anónimo
            if (parametros != null)
            {
                foreach (var prop in parametros.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var param = cmd.CreateParameter();
                    string paramName = prop.Name.StartsWith("@") ? prop.Name : $"@{prop.Name}";
                    param.ParameterName = paramName;
                    param.Value = prop.GetValue(parametros) ?? (object)DBNull.Value;
                    cmd.Parameters.Add(param);
                }
            }

            // 4. Ejecutar y mapear a una LISTA
            var lista = new List<T>();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                T obj = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);

                    // Buscar propiedad que coincida con el nombre de la columna (insensible a mayúsculas)
                    var prop = typeof(T).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (prop != null && prop.CanWrite)
                    {
                        object value = reader.GetValue(i);
                        if (value != null && value != DBNull.Value)
                        {
                            // Manejar tipos nulables (ej: int?)
                            Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            try
                            {
                                prop.SetValue(obj, Convert.ChangeType(value, targetType));
                            }
                            catch (InvalidCastException)
                            {
                                // Si hay un error de conversión, se ignora o puedes agregar un log aquí
                            }
                        }
                    }
                }
                lista.Add(obj);
            }

            return lista; // Siempre devuelve una lista (vacía o con elementos)
        }


        /// <summary>
        /// Ejecuta un comando que no devuelve datos (INSERT, UPDATE, DELETE) de forma asíncrona.
        /// Devuelve el número de filas afectadas por la operación.
        /// </summary>
        /// <param name="sentenciaSQL">La sentencia SQL a ejecutar.</param>
        /// <param name="parametros">Objeto anónimo con los parámetros de la consulta.</param>
        /// <returns>Número de filas afectadas.</returns>
        public async Task<int> EjecutarComandoAsync(string sentenciaSQL, object parametros = null)
        {
            // 1. Asegurar conexión
            if (this.conexion == null || this.conexion.State != ConnectionState.Open)
            {
                this.Conectar();
            }

            // 2. Crear comando independiente (no interfiere con this.comando)
            using var cmd = factory.CreateCommand();
            cmd.Connection = this.conexion;
            cmd.CommandText = sentenciaSQL;
            cmd.CommandType = CommandType.Text;

            // 3. Asignar transacción si existe una activa
            if (this.transaccion != null)
            {
                cmd.Transaction = this.transaccion;
            }

            // 4. Mapear parámetros desde el objeto anónimo (misma lógica que tu EjecutarConsultaAsync)
            if (parametros != null)
            {
                foreach (var prop in parametros.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var param = cmd.CreateParameter();
                    string paramName = prop.Name.StartsWith("@") ? prop.Name : $"@{prop.Name}";
                    param.ParameterName = paramName;
                    param.Value = prop.GetValue(parametros) ?? (object)DBNull.Value;
                    cmd.Parameters.Add(param);
                }
            }

            // 5. Ejecutar y retornar el número de filas afectadas
            return await cmd.ExecuteNonQueryAsync();
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

    }
}
