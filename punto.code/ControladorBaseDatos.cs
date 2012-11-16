using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace punto.code
{
	public class ControladorBaseDatos
	{
		private string _server;
		private string _database;
		private string _user;
		private string _password;

		public ControladorBaseDatos ()
		{
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			this.Server = config.AppSettings.Settings["BdServer"].Value;
			this.Database = config.AppSettings.Settings["BdDatabase"].Value;
			this.User = config.AppSettings.Settings["BdUser"].Value;
			this.Password = config.AppSettings.Settings["BdPassword"].Value;

		}

		public bool AgregarProductosBd (Producto registro)
		{
		
				
				IDbConnection dbcon = this.ConectarBd();
				
				IDbCommand dbcmd = dbcon.CreateCommand();
				string sql =
					"INSERT INTO productos (codigobarra,nombre,precio_venta,nombre_familia,pesable,vigente) " +
						"VALUES ("+registro.Codigobarra+",'"+registro.Nombre+"','"+registro.Precioventa+"','"+registro.Familia+"','"+registro.Pesable+"','"+registro.Vigente+"');";
		
				dbcmd.CommandText = sql;
				IDataReader reader = dbcmd.ExecuteReader();
			
	
				dbcmd.Dispose();
				dbcmd = null;
				
				this.DesconectarBd(dbcon);
	
			
			return false;
		}
		public bool AgregarVentaDetalle (DetalleVenta registro)
		{
			
			
			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"INSERT INTO venta_detalle(idventa_detalle,cantidad,precio_venta,fecha_hora) " +
					"VALUES ("+registro.CODIGOVENTA+",'"+registro.CANTIDAD+"','"+registro.PRECIOVENTA+"','"+registro.FECHA+"');";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			
			
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			
			return false;
		}


		public bool ExisteRegistroProductosBd (int codigob)
		{
			IDbConnection dbcon = this.ConectarBd ();
			
			IDbCommand dbcmd = dbcon.CreateCommand ();
			string sql;
				sql =
					"SELECT codigobarra " +
					"FROM productos " +
					"WHERE codigobarra='"+ codigob + "';";
				dbcmd.CommandText = sql;



			IDataReader reader = dbcmd.ExecuteReader();
			bool existe = reader.Read();
	
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return existe;
		}
	/*	public List<Producto> ObtenerProductosBd (int codigoB)
		{
			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"SELECT codigobarra,nombre,precio_venta,nombre_familia,pesable,vigente " +
					"FROM productos " +
					"WHERE codigobarra='"+codigoB+"'";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			List<Producto> product = new List<Producto>();
			while(reader.Read()) {
				product.Add(new Producto( ((int) reader["codigobarra"]),((string) reader["nombre"]),((int) reader["precio_venta"]),((string) reader["nombre_familia"]),((bool) reader["pesable"]),((bool) reader["vigente"])));
			}
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return product;
		}
  */   
		public bool ActualizarProductoBd (int CodigoB,int PrecioV, string Vigente)
		{
						
			IDbConnection dbcon = this.ConectarBd();
				
				IDbCommand dbcmd = dbcon.CreateCommand();
				string sql =
					"UPDATE productos " +
						"SET precio_venta='"+PrecioV+"'," +
					"vigente='"+Vigente+"'" +
					"WHERE codigobarra="+CodigoB+";";

				dbcmd.CommandText = sql;
				int res = dbcmd.ExecuteNonQuery();
				// clean up
				dbcmd.Dispose();
				dbcmd = null;
				
				this.DesconectarBd(dbcon);
				
			return true;
		}
		public int ObtenerBoleta ()
		{
			int precio=0;
			
			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"SELECT idventa_detalle " +
					"FROM venta_detalle " +
					"order by idventa_detalle DESC limit 1";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			
			while(reader.Read()) {
				precio =   (int)reader["idventa_detalle"];

			}
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return precio+1;
		}
		public string[] ObtenerProductosBd (int codigoB)
		{
			string[] precio = new string[2];

			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"SELECT precio_venta,vigente " +
					"FROM productos " +
					"WHERE codigobarra='"+codigoB+"'";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
		
			while(reader.Read()) {
				precio[0] =  (string) reader["precio_venta"];
				precio[1] =  (string) reader["vigente"];

				//bool vig = reader["vigente"];
			}
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return precio;
		}

		public string[] ObtenerUsuarioContraseñaBd (string usuario) 
		{
			string[] usuarioContraseña = new string[2];
			
			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			
			string sql =
				"SELECT userlogin, userpass " +
					"FROM usuarios "+
					"WHERE userlogin='"+usuario+"'";;
			
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			
			while(reader.Read()) {
				usuarioContraseña[0] = (string) reader["userlogin"];
				usuarioContraseña[1] = (string) reader["userpass"];
				
			}
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			return usuarioContraseña;
		}

		public List<Produc> ObtenerProductosVenta (int codigoB)
		{

			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"SELECT nombre,precio_venta " +
					"FROM productos " +
					"WHERE codigobarra='"+codigoB+"'";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			List<Produc> productos = new List<Produc>();
			while(reader.Read()) {
				productos.Add(new Produc( (string) reader["nombre"],(string) reader["precio_venta"]));
			}
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return productos;
		}

		public List<FamiliaProducto> ObtenerFamiliasBd ()
		{
			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"SELECT nombre " +
					"FROM familia_prod " +
					"ORDER BY nombre ASC";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			List<FamiliaProducto> familias = new List<FamiliaProducto>();
			while(reader.Read()) {
				familias.Add(new FamiliaProducto( (string) reader["nombre"]));
			}
			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return familias;
		}

		public bool AgregarFamiliaBd (FamiliaProducto familia)
		{
			if (!this.ExisteFamiliaBd(familia))
			{
				IDbConnection dbcon = this.ConectarBd();
				
				IDbCommand dbcmd = dbcon.CreateCommand();
				string sql =
					"INSERT INTO familia_prod (nombre)" +
						"VALUES ('"+familia.Nombre+"')";
				dbcmd.CommandText = sql;
				int res = dbcmd.ExecuteNonQuery();
				
			
				dbcmd.Dispose();
				dbcmd = null;
				
				this.DesconectarBd(dbcon);
				
				return true;
			}
			return false;
		}
		public bool ExisteFamiliaBd (FamiliaProducto Familia)
		{
			IDbConnection dbcon = this.ConectarBd();
			
			IDbCommand dbcmd = dbcon.CreateCommand();
			string sql =
				"SELECT nombre " +
					"FROM familia_prod " +
					"WHERE nombre='"+Familia.Nombre+"'";
			dbcmd.CommandText = sql;
			IDataReader reader = dbcmd.ExecuteReader();
			bool existe = reader.Read();

			reader.Close();
			reader = null;
			dbcmd.Dispose();
			dbcmd = null;
			
			this.DesconectarBd(dbcon);
			
			return existe;
		}
		public bool ConfiguracionCorrectaBd
		{
			get {
				bool conectado = true;
				
				try{
					IDbConnection dbcon = this.ConectarBd();
					if (dbcon.State != ConnectionState.Open)
					{
						//nombre servidor, usuario o contraseña incorrecta
						conectado = false;
					}
					this.DesconectarBd(dbcon);
				}
				catch(Exception e)
				{
					conectado = false;
				}
				
				return conectado;
			}
		}
		
		public void GuardarConfiguracionBd ()
		{	
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.AppSettings.Settings["BdServer"].Value = this.Server;
			config.AppSettings.Settings["BdDatabase"].Value = this.Database;
			config.AppSettings.Settings["BdUser"].Value = this.User;
			config.AppSettings.Settings["BdPassword"].Value = this.Password;
			config.Save(ConfigurationSaveMode.Modified);
		}
		private IDbConnection ConectarBd ()
		{
			string connectionString =
				"Server="+this.Server+";" +
					"Database="+this.Database+";" +
					"User ID="+this.User+";" +
					"Password="+this.Password+";" +
					"Pooling=false;" +
					"Allow Zero Datetime=true;";
			
			IDbConnection dbcon = new MySqlConnection(connectionString);
			dbcon.Open();
			return dbcon;
			
		}
		private bool DesconectarBd (IDbConnection dbcon)
		{
			if (dbcon == null)
			{
				return false;
			}
			
			dbcon.Close();
			dbcon = null;
			
			return true;
		}
		
		public string Server
		{
			get {return this._server;}
			set {this._server = value;}
		}
		public string Database
		{
			get {return this._database;}
			set {this._database = value;}
		}
		
		public string User
		{
			get {return this._user;}
			set {this._user = value;}
		}
		
		public string Password
		{
			get {return this._password;}
			set {this._password = value;}
		}
	}
}
