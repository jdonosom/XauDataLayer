![Xau](/images/Nodes.png "Xauro Dev")

[![](https://img.shields.io/github/license/jdonosom/XauDataLayer)](LICENSE.txt)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/w/jdonosom/XauDataLayer)](https://github.com/jdonosom/)
[![GitHub contributors](https://img.shields.io/github/contributors/jdonosom/XauDataLayer)](https://github.com/jdonosom/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/jdonosom/XauDataLayer)](https://github.com/jdonosom/)
[![Nuget Version](https://img.shields.io/nuget/v/Xauro.XauNuget.XauDataLayer)](https://github.com/jdonosom/)
[![Nuget Download](https://img.shields.io/nuget/dt/Xauro.XauNuget.XauDataLayer)](https://github.com/jdonosom/)


# XauDatalayer
XauDataLayer es parte de XauroFramework un framework rapido y lijero para obtener acceso a bases de datos. Según el proveedor de acceso a datos que se especifique. Lo que asegura la comunicación con cualquier base de datos.

## La clase base de BaseDatos()
### Propósito:
La clase BaseDatos(), permite instanciar las funcionalidades de la clase. 
Una vez que la clase es instalaciada lo primero que efectua es la carga de los 
parámetros necesarios para desencriptar los datos conexión a la base de datos. 
Los datos son obtenidos desde el archivo  de configuracion ConfigAccess.json y 
secret.json, desde  variables  de  entorno  del  OS  o  también proporcionados 
directamente a la clase.

## Entradas, datos de configuración y protección de crédenciales
La clase opera con tres entradas de configuración, las que pueden ser proporcionadas
a traves de dos archivos json o desde variables de entorno del sistema operativo o
proporcionadas directamente desde su aplicación.

### Primera entrada de configuración, archivos .json
Archivos secrets.json y ConfigAccess.json, archivo de parametros de encriptacion y 
archivo de configuración de acceso respectivamente.

#### Archivo secret.json
Archivo de configuración de secretos, el archivo contiene la definición
de los siguientes parámetros.

- Frase       : La Frase
- Salt        : La sal
- Algorit     : El algoritmo de encriptación
- Iteraciones : El numero de iteraciones
- Vector      : El vector
- TamanoClave : El Tamaño de la clave

#### contenido del archivo, de secretos (secret.json)
```
{"Frase":"N\u0026\u0026PSeL\u0026#@wMU0Go%lis","Salt":"$2a$12$a97/VFp.sysHWU0B0IyyUe","Algorit":"SHA1","Iteraciones":3,"Vector":"G6g%)LlOXF4@%dkN","TamanoClave":256}
```

#### Archivo ConfigAccess.json
Archivo de configuración de acceso, el archivo contiene la definición de los
siguientes parámetros.

- Adonet        : La clase de conexión con el motor de base de datos.
- Server        : La IP o HOST del servidor de base de datos.
- Port          : El puerto de comunicación con el motor. (si es cero utiliza el puerto por defecto)
- DataBase      : El nombre de la base de datos.
- User          : El nombre de usuario de la base de datos.
- Password      : La contraseña del usuario de la base de datos.

#### contenido del segundo archivo, endpoint y credenciales
```
{"Adonet":{"_Adonet":"System.Data.SqlClient","Encriptado":false},"Server":{"_Server":".","_Port":"0","Encriptado":false},"Database":{"_Database":"lTbwN1L6TdH9NoYIiQGK6g==","Encriptado":true},"User":{"_User":"huHANxisiRQAn8qX9g/SZw==","Encriptado":true},"Password":{"_Password":"q5jTQlGvo5ME\u002BpurJnJF6Q==","Encriptado":true}}
```

Vista hermosa del .json (solo para la conprención del contenido)
```
{
    "Frase": "N\u0026\u0026PSeL\u0026#@wMU0Go%lis",
    "Salt": "$2a$12$a97/VFp.sysHWU0B0IyyUe",
    "Algorit": "SHA1",
    "Iteraciones": 3,
    "Vector": "G6g%)LlOXF4@%dkN",
    "TamanoClave": 256
}
```

Vista hermosa del .json (solo para la conprención del contenido)
```
{
    "Adonet": {
        "_Adonet": "System.Data.SqlClient",
        "Encriptado": false
    },
    "Server": {
        "_Server": ".",
        "_Port": "0",
        "Encriptado": false
    },
    "Database": {
        "_Database": "lTbwN1L6TdH9NoYIiQGK6g==",
        "Encriptado": true
    },
    "User": {
        "_User": "huHANxisiRQAn8qX9g/SZw==",
        "Encriptado": true
    },
    "Password": {
        "_Password": "q5jTQlGvo5ME\u002BpurJnJF6Q==",
        "Encriptado": true
    }
}
```

> [¡Nota!]
>
> La propiedad Encriptado indica si el dato se encuentra encriptado o no, de esta forma
> la clase sabe si debe desencriptar el dato antes de realizar una conexión.
>

### Seguna entrada de configuración, variables de entorno
En caso de usar una variable de entrono solo es necesario definir las variables de entorno y 
pasar los nombres de las variables durante la instanciación de la clase
**BaseDatos(secret, credencials)**.

El formato y el contenido de las variables es identico a lo especificado anteriormente,
la diferencia es de donde se obtienen los datos.

### Tercera entrada de configuración, paso de variables de programa
En caso de necesitar la definición de los parámetros directamente desde la aplicación es
posible pasar los datos directamente al constructor de la clase. La siguiente lista corresponden
a los parámetros necesarios para la instanciación.

- Server   : La IP o Host del servidor de base de datos.
- Port     : Puerto de conexión del servidor.
- User     : Nombre de usuario de la base de datos.
- Password : Contraseña del usuario
- Provider : Cadena especifica para un proveedor de base de datos ej. "System.Data.SqlClient".
- DataBase : Nombre de la base de datos.

### Sintaxis: 
```	
BaseDatos();

or

BaseDatos(string secret, string credencials);

or

BaseDatos(string server
	, string port
	, string userName
	, string password
	, string provider
	, string dataBase);
```
### Ejemplo 1
En el ejemplo, se obtienen los datos de configuración desde los archivos json.
```
public class UsoBaseDatos
{
    private readonly BaseDatos db;

    public UsoBaseDatos()
    {
        db = new BaseDatos db = new BaseDatos();
    }
	
    public decimal ObtenerValorUSD(string Fecha)
    {
        decimal ValorUSD = 0;
        // Los parámetros se leeran desde el archivo de configuración 
        // o desde variables de entorno
        // 
        BaseDatos db = new BaseDatos(); 
        try                             
        {
            db.Conectar();
            ...
            ...
            ...
        }
        catch()
        {
            ...
            ...
        }
        finaly
        {
            db.Desconectar();
        }
        return ValorUSD;
    }
	
}

```

### Ejemplo 2
En el ejemplo, se obtienen los datos desde variables de entorno.
```
public class UsoBaseDatos
{
    private readonly BaseDatos db;

    public UsoBaseDatos()
    {
        db = new BaseDatos db = new BaseDatos("ENVIRON_VAR_SECRET", "ENVIRON_VAR_CREDENCIAL");
    }

    public decimal ObtenerValorUSD(string Fecha)
    {
        decimal ValorUSD = 0;
        try
        {
            db.Conectar();
            ...
            ...
            ...
        }
        catch()
        {
            ...
            ...
        }
        finaly
        {
            db.Desconectar();
        }
        return ValorUSD;
    }
}
```

### Ejemplo 3
En el ejemplo, se obtienen los datos desde variables de programa.
```
public class UsoBaseDatos
{
    private readonly BaseDatos db;

    public UsoBaseDatos()
    {
        string servidor = "127.0.0.1";
        string port = "3306";
        string userName = "root";
        string password = "rootsecret";
        string proveedor = "MySql.Data.MySqlClient";
        string dataBase  = "test100";

        db = new BaseDatos(servidor, port, userName, password, proveedor, dataBase);  
    }

    public decimal ObtenerValorUSD(string Fecha)
    {
	    decimal ValorUSD = 0;
	    try
	    {
		    db.Conectar();
		    ...
		    ...
		    ...
	    }
	    catch()
	    {
		    ...
		    ...
	    }
        finaly
        {
            db.Desconectar();
        }
	    return ValorUSD;
    }
}
```


## Metodos públicos
A continuación en tabla se muestra la lista de metodos disponibles de la clase **XauDataLayer.**

| Métodos | Descripción|
|---------|------------|
|Conectar() | Inicia una conexión con la base de datos.|
|Desconectar()| Termina una coneción realizada previamente con Conectar()|
|CrearComando(comandoSql) | Crea un comando SQL (Store Procedure o Query) a ejecutar. |
|EjecutarComando() | Ejecuta un el comando previamente especificado por CrearComando() y que no espera una respuesta |
|EjecutarConsulta() | Ejecuta un el comando y retorna una clase DbDataReader()|
|EjecutarEscalar() | Ejecuta un comando y retorna un escalar |
|AsignarParametroBoolean(nombre, valor)| Especifica el nombre y el valor del parametro booleano.|
|AsignarParametroCadena(nombre, valor)| Especifica el nombre y el valor del parametro cadena.|
|AsignarParametroDecimal(nombre, valor)| Especifica el nombre y el valor del parametro decimal.|
|AsignarParametroDouble(nombre, valor)| Especifica el nombre y el valor del parametro double.|
|AsignarParametroEntero(nombre, valor)| Especifica el nombre y el valor del parametro int.|
|AsignarParametroFecha(nombre, valor)| Especifica el nombre y el valor del parametro dateTime.|
|AsignarParametroFloat(nombre, valor)| Especifica el nombre y el valor del parametro float.|
|AsignarParametroImage(nombre, valor)| Especifica el nombre y el valor del parametro image.|
|AsignarParametroNulo(nombre)| Especifica el nombre parametro null.|
|CancelarTransaccion()| Cancela la transacción un rollback de los cambios es realizado.|
|ComenzarTransaccion()| Inicializa una transcación. |
|ConfirmarTransaccion() | Confirma la transacción y todos los cambios son grabados|


## Declaración de clase (Constructor)
### Propósito:
Instanciar la clase para acceder a los metodos que presta la clase.
### Sintaxis 1:
```
BaseDatos DB = new BaseDatos();
```
En este caso la clase instanciada entendera que las entradas serán proporcionadas a traves
de los archivos .json de configuración.
### Sintaxis 2:
```
BaseDatos DB = new BaseDatos(VAR_SECRET, VAR_CREDENCIAL);
```
En este caso la clase instanciada entendera que las entradas serán proporcionadas a traves
de las variables de entorno proporcionadas.

### Sintaxis 3:
```
public BaseDatos(servidor, port, userName, password, provider, dataBase);
```
En este caso la clase entendera que las entradas serán proporcionadas sin ningun 
tipo de encriptación y proporcionadas a traves variables definidas dentro de la aplicación.

### Ejemplos
```
// Lee los archivos ConfigAccess.json y secrets.json
BaseDatos DB = new BaseDatos();

or

// Lee las variables de entorno "SGS_SEC" y "SGS_CRE" definidas en el OS
BaseDatos DB = new BaseDatos("SGS_SEC", "SGS_CRE");

or 

var server = "127.0.1.1";
var port = 1433;
var userName = "sa";
var password = "secreto";
var provider = "System.Data.SqlClient";
var dataBase = "Ventas";
// Obtiene los datos de conexión desde variable internas de la aplicación
BaseDatos DB = new BaseDatos(server, port, userName, password, provider, dataBase);
```




## Metodo Conectar()
### Propósito:
El metodo **Conectar()**, permite realizar la conexión con la base de datos.
### Sintaxis
```
DB.Conectar();
```

### Ejemplo
Este ejemplo utiliza las variables de entorno para proporcionar la entradas necesarias
para realizar la conexión a la base de datos y desencriptación de las credenciales.
```
public class BodegaService : Bodega, IBodega
{
    BaseDatos DB = new BaseDatos("VAR_VENTA_SECRET", "VAR_VENTA_CREDENCIALES");

    public Bodega Get(int IdBodega)
    {
        try
        {
            DB.Conectar();
    
            DB.CrearComando("BodegasSelProc @idBodega");
            DB.AsignarParametroEntero("@IdBodega", IdBodega);
    
            DbDataReader dr = DB.EjecutarConsulta();
            ...
            ... 
            ...
            return _bodega
        }
        catch (BaseDatosException ex)
        {
            throw new ReglasNegocioException(ex.Message.ToString());
        }
        finally
        {
            DB.Desconectar();
        }
    }
}

```

### Ejemplo
Este ejemplo utiliza los archivos de configuración para proporcionar la entradas necesarias
para realizar la conexión a la base de datos y desencriptación de las credenciales.
```
    BaseDatos DB = new BaseDatos();
    try
    {
        DB.Conectar();
        DB.CrearComando("FoliosSelProc @idFolios, @Folio");
        DB.AsignarParametroEntero("@IdFolios", IdFolios);
        DB.AsignarParametroEntero("@Folio", Folio);
        DB.Desconectar();
    }
```

## Metodo Desconectar()
### Propósito:
El metodo **Desconectar()**, permite realizar la desconexión de la base de datos después de
realizar una consulta o ejecutar un comando cerrando la comunicación con el motor de base de
datos.
### Sintaxis
```
DB.Desconectar();
```
### Ejemplo
```
    ...
    ...
    catch (BaseDatosException ex)
    {
        throw new ReglasNegocioException(ex.Message.ToString());
    }
    finally
    {
        DB.Desconectar();
    }
    ...
    ...
```
## Metodo CrearComando(comandoSql) 
### Propósito:
El metodo **CrearComando()** Crea un comando SQL (Store Procedure o Query) que será ejecutado.

### Sintaxis
```
DB.CrearComando(strSQL);
```
Donde strSQL es una cadena de caracteres que contienen un procedimiento almacenado o Query
y la lista de parámetros.

### Ejemplos
```
    int idFolio = 39;
    int Folio = 1210;

    DB.Conectar();
    DB.CrearComando("FoliosDelProc @IdFolios, @Folio");
    DB.AsignarParametroEntero("@IdFolios", idFolios);
    DB.AsignarParametroEntero("@Folio", Folio);
    DB.EjecutarComando();

    or 

    int Folio = 1150;
    DB.Conectar();
    DB.CrearComando("DELETE FROM Facturas WHERE NroFactuta = @Folio");
    DB.AsignarParametroEntero("@Folio", Folio);
    DB.EjecutarComando();

```
## Metodo EjecutarComando()
### Propósito:
El método **EjecutarComando()** permite ejecutar un comando SQL que no requiera obtener 
una respuesta desde el motor de base de datos. Como por ejemplo un comando delete o update.
### Sintaxis:
```
DB.EjecutarComando();
```

### Ejemplos
```
DB.CrearComando("DELETE FROM Pagos WHERE Fecha = '20240701'");
DB.EjecutarComando();

or

DB.CrearComando("LimpiarPagos @Fecha");
DB.AsignarParametroCadena("@Fecha", '20240701');
DB.EjecutarComando();
```

## Metodo EjecutarConsulta()
### Propósito:
El metodo **EjecutarConsulta()** permite ejecutar un comando SQL y obtener un set de datos 
desde el motor de base de datos.
### Sintaxis:
```
var RutEmpresa = 76669800
DB.CrearComando("EmpresaSelProc @RutEmpresa");
DB.AsignarParametroEntero("@RutEmpresa", RutEmpresa);

DbDataReader dr = DB.EjecutarConsulta();

or 

var RutEmpresa = 76669800
DB.CrearComando("SELECT * FROM Empresas WHERE Rut = @RutEmpresa");
DB.AsignarParametroEntero("@RutEmpresa", RutEmpresa);

DbDataReader dr = DB.EjecutarConsulta();
```

## Metodo EjecutarEscalar()
### Propósito:
El metodo **EjecutarEscalar()** permite ejecutar un comando SQL y obtener un escalar desde 
el motor de base de datos.
### Sintaxis:
```
int escalar = (int)DB.EjecutarEscalar();
```
### Ejemplo
En este ejemplo se utiliza un escalar "FolioBoleta" devuelto desde el procedimiento almacenado
el que es utilizado para almacenar los detalles de venta correctamente.

```
    DB.Conectar();
    DB.ComenzarTransaccion();

    // Generar Boleta: 
    //
    DB.CrearComando("BoletasUpdProc @Folio, @IdCliente, @Fecha, @Neto, @TotalIva, @Total");

    DB.AsignarParametroDecimal("@Folio", 0);
    DB.AsignarParametroCadena("@IdCliente", current.IdCliente);
    DB.AsignarParametroFecha("@Fecha", current.Fecha);
    DB.AsignarParametroDouble("@Neto", current.Neto);
    DB.AsignarParametroDouble("@TotalIva", current.TotalIva);
    DB.AsignarParametroDouble("@Total", current.Total);

    FolioBoleta = (int)DB.EjecutarEscalar();

    //
    // Generar Detalle Boleta
    //
    int i = 0;
    foreach (BoletasDetalle row in detalles)
    {
        //
        // Grabar el Detalle de la boleta.
        //
        DB.CrearComando("BoletasDetalleUpdProc @Folio, @Item, @IdProducto, @Descripcion, @Cantidad, @ValorUnitario, @TotalItem, @IdUM");
        
        DB.AsignarParametroFloat("@Folio", FolioBoleta);
        DB.AsignarParametroEntero("@Item", row.Item);
        DB.AsignarParametroCadena("@IdProducto", row.IdProducto);
        DB.AsignarParametroCadena("@Descripcion", row.Descripcion);
        DB.AsignarParametroDecimal("@Cantidad", row.Cantidad);
        DB.AsignarParametroDouble("@ValorUnitario", row.ValorUnitario);
        DB.AsignarParametroDouble("@TotalItem", row.TotalItem);
        DB.AsignarParametroCadena("@IdUM", row.IdUM);
        DB.EjecutarComando();
    }
    ...
    ...
    DB.ConfirmarTransaccion();
    DB.Desconectar();
```

### Codigo SQLServer BoletasUpdProc
Un ejemplo real de como enviar el escalar desde SQLServer (Ver ultima lineas)
```
CREATE PROCEDURE [dbo].[BoletasUpdProc]
(
	@Ambiente               varchar(4),
	@Folio              	numeric, 
	@Caja                   char(6),
	@idCliente          	varchar(11), 
	@Operacion              varchar(25),
	@Fecha              	datetime, 
	@Periodo            	char(6), 
	@Estado             	varchar(3), 
	@Neto               	float, 
	@TotalIva           	float, 
	@Exento                 float,
	@Total              	float, 
	@TotalRound             float,
	@Glosa              	varchar(200), 
	@Descuento          	float, 
	@TipoDoc            	int, 
	@Subtotal           	float, 
	@TasaImpIva         	float,
	@TasaComisionCenabast   float, 
	@Observacion        	varchar(80), 
	@CondicionPago      	int,  	
	@Pago                   float, 
	@Vuelto                 float, 
	@TrackID                int,
	@XML                    nvarchar(max),
	@Usuario            	varchar(25),
	@EstadoSII              varchar(6),
	@RutEmpresa             varchar(11)
)
AS
BEGIN
	 SET NOCOUNT ON
	 BEGIN TRANSACTION
	 
	 DECLARE @FolioBoleta int

	 If( SELECT 1 FROM Boletas WHERE Folio = @Folio AND Ambiente = @Ambiente ) = 1
	 BEGIN
		 UPDATE Boletas
		    SET --Ambiente		      = @Ambiente
			   --,Folio                 = @Folio,
			   Caja					  = @Caja,
			   idCliente              = @idCliente
			   ,Operacion		      = @Operacion
			   ,Fecha                 = @Fecha
			   ,Periodo               = @Periodo
			   ,Estado                = @Estado
			   ,Neto                  = @Neto
			   ,TotalIva              = @TotalIva
			   ,Exento				  = @Exento
			   ,Total                 = @Total
			   ,TotalRound            = @TotalRound
			   ,Glosa                 = @Glosa
			   ,Descuento             = @Descuento
			   ,TipoDoc               = @TipoDoc
			   ,Subtotal              = @Subtotal
			   ,TasaComisionCenabast  = @TasaComisionCenabast
			   ,TasaImpIva			  = @TasaImpIva
			   ,Observacion			  = @Observacion
			   ,CondicionPago		  = @CondicionPago
			   ,Pago				  = @Pago
			   ,Vuelto				  = @Vuelto
			   ,TrackID				  = @TrackID
			   ,XML					  = @XML
			   ,Usuario				  = @Usuario
			   ,EstadoSII			  = @EstadoSII

		  WHERE Folio = @Folio AND Ambiente = @Ambiente

		  SET @FolioBoleta = @Folio

		  IF(@@error!=0)
		  BEGIN
			 RAISERROR  20001 'BoletasUpdProc: No pudo se actualizar la tabla Boletas'
			 ROLLBACK TRAN
			 RETURN(1)
		  END
	 END
	 ELSE
	 BEGIN
		DECLARE @Beneficiario VARCHAR(100);
		SET @FolioBoleta = (SELECT Top(1) Folio FROM [DTE].[DBO].[Folios] WHERE (TD = @TipoDoc) AND (FechaExpiracion >= CONVERT(DATE, GETDATE())) AND (Folio BETWEEN D AND H) AND (RE = @RutEmpresa) order by FechaExpiracion asc)
		SET @Beneficiario = (SELECT Nombres + ' ' + ApePaterno FROM Beneficiarios WHERE Rut = @idCliente);
		SET @Glosa = 'Venta a :' + @idCliente + ' - ' + @Beneficiario;

		 INSERT INTO Boletas( 
				  Folio
				 ,Ambiente
				 ,Caja
				 ,idCliente
				 ,Operacion
				 ,Fecha
				 ,Periodo
				 ,Estado
				 ,Neto
				 ,TotalIva
				 ,Exento
				 ,Total
				 ,TotalRound
				 ,Glosa
				 ,Descuento
				 ,TipoDoc
				 ,Subtotal
				 ,TasaComisionCenabast
				 ,TasaImpIva
				 ,Observacion
				 ,CondicionPago
				 ,Pago
				 ,Vuelto
				 ,TrackID
				 ,XML 
				 ,Usuario
				 ,EstadoSII)
				 VALUES (
				  @FolioBoleta
				 ,@Ambiente
				 ,@Caja
				 ,@idCliente
				 ,@Operacion
				 ,@Fecha
				 ,@Periodo
				 ,@Estado
				 ,@Neto
				 ,@TotalIva
				 ,@Exento
				 ,@Total
				 ,@TotalRound 
				 ,@Glosa
				 ,@Descuento
				 ,@TipoDoc
				 ,@Subtotal
				 ,@TasaComisionCenabast
				 ,@TasaImpIva
				 ,@Observacion
				 ,@CondicionPago
				 ,@Pago
				 ,@Vuelto
				 ,@TrackID
				 ,@XML
				 ,@Usuario
				 ,@EstadoSII)
		 IF(@@error != 0)
		 BEGIN
			 RAISERROR 20000 'BoletasUpdProc : No puedo insertar el valor registro en la tabla.'
			 ROLLBACK TRANSACTION
			 RETURN(1)
		 END

		 UPDATE [DTE].[DBO].[Folios] SET Folio = @FolioBoleta + 1  WHERE (TD = @TipoDoc) AND (FechaExpiracion >= CONVERT(DATE, GETDATE())) AND (Folio BETWEEN D AND H) AND (RE = @RutEmpresa) AND Folio = @FolioBoleta
		 
	 END
	 COMMIT TRANSACTION
	 SELECT @FolioBoleta AS FolioBoleta;
	 SET NOCOUNT OFF
END
```

# ASIGNACIÓN DE PARAMETROS

## Metodo AsignarParametroBoolean(nombre, valor)
### Propósito:
El metodo **AsignarParametroBoolean()**, permite pasar parámetros de tipo boolean al comando creado con **CrearComando()**

### Sintaxis
```
DB.AsignarParametroBoolean("<@NameParameter>", <bool>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo

```
int Id = 10021;
bool Vencido = false;

DB.Conectar();
DB.CrearComando($"{db}ForwareSelProc @Id, @Vencido");
DB.AsignarParametroEntero("@Id", Id);
DB.AsignarParametroBoolean("@Vencido", Vencido);          <<-- Asignación booleano
DbDataReader dr = DB.EjecutarConsulta();
```

## Metodo AsignarParametroCadena(nombre, valor)
### Propósito:
El metodo **AsignarParametroCadena()** permite pasar datos de tipo cadena al comando.
### Sintaxis
```
DB.AsignarParametroCadena(<@NameParameter>, <string>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.<br>
Value          : Valor del parámetro.
```

### Ejemplo
```
DB.AsignarParametroCadena("@Nombre", "Pepe Luna Park");
```


## Metodo AsignarParametroDecimal(nombre, valor)
### Propósito:
El metodo **AsignarParametroDecimal()** permite pasar datos de tipo decimal al comando.
### Sintaxis
```
DB.AsignarParametroDecimal(<@NameParameter>, <decimal>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo
```
DB.AsignarParametroDecimal("@TasaIva", 0.19);
```


## Metodo AsignarParametroDouble(nombre, valor)
### Propósito:
El metodo **AsignarParametroDouble()** permite pasar datos de tipo double al comando.
### Sintaxis
```
DB.AsignarParametroDouble(<@NameParameter>, <double>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo
```
DB.AsignarParametroDouble("@GanaciaTotal", 9223372036854775807);
```

## Metodo AsignarParametroEntero(nombre, valor)
### Propósito:
El metodo **AsignarParametroEntero()** permite pasar datos de tipo int al comando.
### Sintaxis
```
DB.AsignarParametroEntero(<@NameParameter>, <int>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo
```
DB.AsignarParametroEntero("@DiasAlVencimieto", 265);
```

## Metodo AsignarParametroFecha(nombre, valor)
### Propósito:
El metodo **AsignarParametroFecha()** permite pasar datos de tipo DateTime al comando.
### Sintaxis
```
DB.AsignarParametroFecha(<@NameParameter>, <DateTime>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo
```
DB.AsignarParametroFecha("@FechaNacimiento", DateTime.Now );
```



## Metodo AsignarParametroFloat(nombre, valor)
### Propósito:
El metodo **AsignarParametroFloat()** permite pasar datos de tipo float al comando.
### Sintaxis
```
DB.AsignarParametroFloat(<@NameParameter>, <float>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo
```
DB.AsignarParametroFloat("@PI", 3.141592653589);
```


AsignarParametroImage(nombre, valor)

## Metodo AsignarParametroFloat(nombre, valor)
### Propósito:
El metodo **AsignarParametroImage()** permite pasar datos de tipo byte[] al comando.
### Sintaxis
```
DB.AsignarParametroImage(<@NameParameter>, <byte[]>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
Value          : Valor del parámetro.
```

### Ejemplo
Este ejemplo muestra como enviar un parámetro tipo Image, utilizando la función ImageToByteArray() para convertirla
en un arreglo de bytes.
```
DB.AsignarParametroImage("@Foto", ImageToByteArray("\\Server\imagenes\\PepeLunaPark.jpg"));


public byte[] ImageToByteArray(System.Drawing.Image imageIn)
{
   using (var ms = new MemoryStream())
   {
      imageIn.Save(ms,imageIn.RawFormat);
      return  ms.ToArray();
   }
}
```




## Metodo AsignarParametroFloat(nombre, valor)
### Propósito:
El metodo **AsignarParametroImage()** permite pasar datos de tipo byte[] al comando.
### Sintaxis
```
DB.AsignarParametroNulo(<@NameParameter>);

Parámetros:
@NameParameter : Cadena con el nombre del parametro.
```

### Ejemplo
Este ejemplo muestra como pasar un parámetro nulo al comando.

```
DB.AsignarParametroNulo("@XML");
```


# Transacciones

## Metodo ComenzarTransaccion()
### Propósito:
El metodo **ComenzarTransaccion()** permite iniciar una transacción en la base de datos.
### Sintaxis
```
DB.ComenzarTransaccion();
```

## Metodo CancelarTransaccion()
### Propósito:
El metodo **ComenzarTransaccion()** permite cancelar una transacción previamente inicializada en la base de datos.
### Sintaxis
```
DB.CancelarTransaccion();
```

## Metodo ConfirmarTransaccion()
### Propósito:
El metodo **ConfirmarTransaccion()** permite confirmar una transacción previamente inicializada en la base de datos.
### Sintaxis
```
DB.ConfirmarTransaccion();
```

### Ejemplo
Ejemplo de como utilizar correctamente las transacciones.

```
public bool UpdateSales(ref int FolioBoleta, int IdBodega, string RUTEmpresa, List<int[]> vencidos)
{
    Boolean lRet = false;
    try
    {
        DB.Conectar();
        DB.ComenzarTransaccion();
        
        // Generar Boleta: 
        //
        DB.CrearComando("BoletasUpdProc @Folio, @Fecha, @Neto, @TotalIva, @Total");
        
        DB.AsignarParametroDecimal("@Folio", 0);
        DB.AsignarParametroFecha("@Fecha", current.Fecha);
        DB.AsignarParametroDouble("@Neto", current.Neto);
        DB.AsignarParametroDouble("@TotalIva", current.TotalIva);
        DB.AsignarParametroDouble("@Total", current.Total);
        FolioBoleta = (int)DB.EjecutarEscalar();
        
        //
        // Generar Detalle Boleta, actualizacion de inventarios
        //
        int i = 0;
        foreach (BoletasDetalle row in detalles)
        {
            //
            // Grabar el Detalle de la boleta.
            //
            DB.CrearComando("BoletasDetalleUpdProc @Folio, @Item, @IdProducto, @Descripcion, @Cantidad, @ValorUnitario,  @TotalItem");

            DB.AsignarParametroFloat("@Folio", FolioBoleta);
            DB.AsignarParametroEntero("@Item", row.Item);
            DB.AsignarParametroCadena("@IdProducto", row.IdProducto);
            DB.AsignarParametroCadena("@Descripcion", row.Descripcion);
            DB.AsignarParametroDecimal("@Cantidad", row.Cantidad);
            DB.AsignarParametroDouble("@ValorUnitario", row.ValorUnitario);
            DB.AsignarParametroDouble("@TotalItem", row.TotalItem);
            DB.EjecutarComando();

            // Registrar el movimiento en Inventarios de movimientos
            //
            DB.CrearComando("KardexMovimientosUpdSaldoProc @IdTipoMovimiento, @Imputacion, @Glosa, @IdProducto, @Cantidad, @IdBodega, @Usuario");
            DB.AsignarParametroCadena("@IdTipoMovimiento", "VTSAL");
            DB.AsignarParametroCadena("@Imputacion", "SAL");
            DB.AsignarParametroCadena("@Glosa", $"Salida por venta {current.Fecha} Boleta de venta N° {FolioBoleta}.");
            DB.AsignarParametroCadena("@IdProducto", row.IdProducto);
            DB.AsignarParametroDouble("@Cantidad", (double)row.Cantidad);
            DB.AsignarParametroEntero("@IdBodega", IdBodega);
            DB.AsignarParametroCadena("@Usuario", current.Usuario);
            DB.EjecutarComando();
        }
        DB.ConfirmarTransaccion();
    }
    catch (Exception e)
    {
        lRet = false;
        DB.CancelarTransaccion();
        
        throw new ReglasNegocioException(e.Message);
    }
    finally
    {
        DB.Desconectar();
    }
    return lRet;
}
```


## Metodo Ejemplo completo de implementación de una clase
### Ejemplo
El ejemplo muestra la implementación de las llamadas a la clase XauDataLayer y la  implementación de  los
metodos Get, Delete y Update (incluye el insert).

```
namespace Inventarios.BL
{
    public partial class clsBodegas: Bodegas
    {
        readonly BaseDatos DB = new BaseDatos();
        
        Bodegas current = null;
        #region Propiedades;
        string toxml;
        int count;
        public int Count
        {
            get { return count; }
        }
        #endregion
        
        #region Tipo Datos
        #endregion
        
        #region Metodos Publicos
        
        private string usuario;
        private string host;
        
        
        public string Host
        {
            get { return host; }
            set { host = value; }
        }
        
        public clsBodegas()
        {
            this.usuario = Credenciales.Usuario;
            this.host = Credenciales.Host;
        }
        public void Clear()
        {
            this.IdBodega = 0;
            this.Descripcion = "";
            this.Direccion = "";
            this.Telefono = "";
            this.Usuario = "";
        }

        public List<Bodegas> Get(System.Int32 IdBodega)
        {
            var oLst = new List<Bodegas>();
            DB.Conectar();
            try
            {
                DB.CrearComando("BodegasSelProc @idBodega");
                DB.AsignarParametroEntero("@IdBodega", IdBodega);

                DbDataReader dr = DB.EjecutarConsulta();

                DataTable dt = new DataTable();
                dt.TableName = MethodBase.GetCurrentMethod().DeclaringType.Name;
                dt.Load(dr);
                this.count = dt.Rows.Count;

                if (this.count > 0)
                {
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    dt.WriteXml(writer, XmlWriteMode.WriteSchema);
                    this.toxml = writer.ToString();
                }

                DataTableReader reader = new DataTableReader(dt);

                if (reader == null)
                {
                    this.count = 0;
                    return null;
                }
                while (reader.Read())
                {
                    try
                    {
                        current = ReadDataRow(reader);
                        oLst.Add(current);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                reader.Close();
                return oLst;
            }
            catch (BaseDatosException ex)
            {
                throw new ReglasNegocioException(ex.Message.ToString());
            }
            finally
            {
                DB.Desconectar();
            }
        }

         public Boolean Delete(System.Int32 idBodega)
         {
             Boolean lRet = false;

             if (this.Exists(idBodega))
             {
                 try
                 {
                     DB.Conectar();
                     DB.CrearComando("BodegasDelProc @IdBodega");
                     DB.AsignarParametroEntero("@IdBodega", IdBodega);

                     DB.EjecutarComando();
                     lRet = true;
                 }
                 catch (BaseDatosException)
                 {
                     lRet = false;
                     throw new ReglasNegocioException("Error al acceder a la base de datos para eliminar el registro.");
                 }
                 catch (ReglasNegocioException)
                 {
                     lRet = false;
                     throw new ReglasNegocioException("Error al eliminar el registro.");
                 }
                 finally
                 {
                     DB.Desconectar();
                 }
             }
             return lRet;
         }

         public Boolean Update()
         {
             Boolean lRet = false;

             try
             {
                 DB.Conectar();
                 DB.CrearComando("BodegasUpdProc @IdBodega, @Descripcion, @Direccion, @Telefono, @Usuario");

                 DB.AsignarParametroEntero("@IdBodega", IdBodega);
                 DB.AsignarParametroCadena("@Descripcion", Descripcion);
                 DB.AsignarParametroCadena("@Direccion", Direccion);
                 DB.AsignarParametroCadena("@Telefono", Telefono);
                 DB.AsignarParametroCadena("@Usuario", Usuario);

                 DB.EjecutarComando();
                 lRet = true;
             }
             catch (BaseDatosException)
             {
                 lRet = false;
                 throw new ReglasNegocioException("Error al acceder a la base de datos para insertar el registro.");
             }
             catch (ReglasNegocioException)
             {
                 lRet = false;
                 throw new ReglasNegocioException("Error al eliminar el cliente.");
             }
             catch (Exception e)
             {
                 lRet = false;
                 throw new ReglasNegocioException(e.Message);
             }
             finally
             {
                 DB.Desconectar();
             }
             return lRet;
        }

        #endregion

        #region Metodos Privados
        private Boolean Exists(System.Int32 idBodega)
        {
            Boolean lRet = false;
            try
            {
                //if (idBodega <= 0) throw new ReglasNegocioException("Id no valido.");
                DB.Conectar();
                DB.CrearComando("BodegasSelProc @IdBodega");
                DB.AsignarParametroEntero("@IdBodega", IdBodega);

                DbDataReader dr = DB.EjecutarConsulta();

                DataTable dt = new DataTable();

                dt.Load(dr);
                this.count = dt.Rows.Count;
                DataTableReader reader = new DataTableReader(dt);
                if (this.count <= 0)
                    return lRet;

                lRet = true;
                reader.Close();
                DB.Desconectar();
            }
            catch (BaseDatosException)
            {
                throw new ReglasNegocioException("Error al acceder a la base de datos, no se pudo validar la existencia del registro.");
            }
            catch (ReglasNegocioException ex)
            {
                throw new ReglasNegocioException("Error a obtener los datos." + ex.Message);
            }
            return lRet;
        }

        private Bodegas ReadDataRow( DataTableReader reader ) 
        {
        
            current = null;
            
            var e = new Bodegas()
            {
                IdBodega = reader.IsDBNull(reader.GetOrdinal("idBodega")) ? 0: reader.GetInt32(reader.GetOrdinal("idBodega")),
                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? "": reader.GetString(reader.GetOrdinal("Descripcion")),
                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? "": reader.GetString(reader.GetOrdinal("Direccion")),
                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? "": reader.GetString(reader.GetOrdinal("Telefono")),
                Usuario = reader.IsDBNull(reader.GetOrdinal("Usuario")) ? "": reader.GetString(reader.GetOrdinal("Usuario")),
            };
            this.current = e;
            
            this.IdBodega = e.IdBodega;
            this.Descripcion = e.Descripcion;
            this.Direccion = e.Direccion;
            this.Telefono = e.Telefono;
            this.Usuario = e.Usuario;
            return(Bodegas)e;
            
        }
        #endregion
    }
}
```

### Licencias
Licencia Open Source para uso no comercial/personal