using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XauCfg
{

    internal static class ParametrosClave
    {
        internal static string Frase { get; set; }
        internal static string Salt { get; set; }
        internal static string Algorit { get; set; }
        internal static int Iteraciones { get; set; }
        internal static string Vector { get; set; }
        internal static int TamanoClave { get; set; }

    }

    public class Parametros
    {
        public string Frase { get; set; }
        public string Salt { get; set; }
        public string Algorit { get; set; }
        public int Iteraciones { get; set; }
        public string Vector { get; set; }
        public int TamanoClave { get; set; }

        public Parametros()
        {
            Frase = ParametrosClave.Frase;
            Salt = ParametrosClave.Salt;
            Algorit = ParametrosClave.Algorit;
            Iteraciones = ParametrosClave.Iteraciones;
            Vector = ParametrosClave.Vector;
            TamanoClave = ParametrosClave.TamanoClave;
        }

        public void Guardar()
        {
            ParametrosClave.Frase = Frase;
            ParametrosClave.Salt = Salt;
            ParametrosClave.Algorit = Algorit;
            ParametrosClave.Iteraciones = Iteraciones;
            ParametrosClave.Vector = Vector;
            ParametrosClave.TamanoClave = TamanoClave;
        }
    }

    #region Clases de configuracion
    public class Adonet
    {
        public string Proveedor { get; set; }
        public bool Encriptado { get; set; }
    }
    public class Server
    {
        public string Nombre { get; set; }
        public string Puerto { get; set; }
        public bool Encriptado { get; set; }
    }

    public class Database
    {
        public string Nombre { get; set; }
        public bool Encriptado { get; set; }
    }
    public class User
    {
        public string Nombre { get; set; }
        public bool Encriptado { get; set; }
    }

    public class Password
    {
        public string Secreto { get; set; }
        public bool Encriptado { get; set; }
    }
    #endregion

    public class Config
    {
        public Adonet Adonet { get; set; }
        public Server Server { get; set; }
        public Database Database { get; set; }
        public User User { get; set; }
        public Password Password { get; set; }
    }

}
