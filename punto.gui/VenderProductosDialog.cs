using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Gtk;
using punto.code;

namespace punto.gui
{

	public partial class VenderProductosDialog : Gtk.Dialog
	{
		public event EventHandler TimeChanged;
		public int CurrentTime;
		
	

		private ControladorBaseDatos db;
		
		public List<Produc> productoventa = new List<Produc>();
		private Gtk.ListStore ventamodel;

		public List<DetalleVenta> listapago = new List<DetalleVenta>();
		private Gtk.ListStore listaventa;

		
		public event EventHandler<EdicionDialogChangedEventArgs> EdicionDialogChanged;
		private bool cambiado = false;
		public	int preciototal=0;
		private string boleta;


		public VenderProductosDialog (Gtk.Window parent) : base ("Vender Productos", parent, Gtk.DialogFlags.DestroyWithParent)
		{

			this.ventamodel = new Gtk.ListStore (typeof (int),typeof (string), typeof (string));

			this.Build ();
			this.db = new ControladorBaseDatos();
			bool correcta = false;

			try {
				correcta = this.db.ConfiguracionCorrectaBd;
			}
			catch (Exception ex)
			{
				correcta = false;
			}
			if (!correcta)
			{
				//mostrar dialog configuracion
				basedatosdialog bdd = new basedatosdialog(this);
				bdd.Run();
				this.db = null;
				this.db = new ControladorBaseDatos();
				
				correcta = false;
				
				try {
					correcta = this.db.ConfiguracionCorrectaBd;
				}
				catch (Exception ex)
				{
					correcta = false;
				}
				
				if(!correcta)
				{
					//Application.Quit();
				}
			}
			Gtk.TreeViewColumn cantidad_column = new Gtk.TreeViewColumn();
			cantidad_column.Title = "Cantidad";
			Gtk.CellRendererText cantidad_cell = new Gtk.CellRendererText();
			cantidad_column.PackStart(cantidad_cell, true);

			Gtk.TreeViewColumn precio_column = new Gtk.TreeViewColumn();
			precio_column.Title = "Precio";
			Gtk.CellRendererText precio_cell = new Gtk.CellRendererText();
			precio_column.PackStart(precio_cell, true);
			
			Gtk.TreeViewColumn nombre_column = new Gtk.TreeViewColumn();
			nombre_column.Title = "Nombre";
			Gtk.CellRendererText nombre_cell = new Gtk.CellRendererText();
			nombre_column.PackStart(nombre_cell, true);

			this.treeview2.AppendColumn(cantidad_column);
			cantidad_column.AddAttribute(cantidad_cell, "text", 0);
			this.treeview2.AppendColumn(nombre_column);
			nombre_column.AddAttribute(nombre_cell, "text", 1);
			this.treeview2.AppendColumn(precio_column);
			precio_column.AddAttribute(precio_cell, "text", 2);
#if DEBUG

			Console.WriteLine(this.db.ObtenerBoleta());
#endif	
				
			int temp=db.ObtenerBoleta();
			boleta=temp.ToString();
			entry2.Text=boleta;
			this.treeview2.Selection.Changed += TreeView2SelectionChanged;

			GLib.ExceptionManager.UnhandledException += ExcepcionDesconocida;
			this.Deletable = true;
		}
		
		public void Destroy ()
		{
#if DEBUG
			Console.WriteLine("Destroy");
#endif
			GLib.ExceptionManager.UnhandledException -= ExcepcionDesconocida;
			EdicionDialogChangedEventArgs args = new EdicionDialogChangedEventArgs(this.cambiado);
			
			base.Destroy();
		}
		
		private void ExcepcionDesconocida (GLib.UnhandledExceptionArgs e)
		{
#if DEBUG
			Console.WriteLine(e.ToString());
#endif
			Dialog dialog = new Dialog("OK", this, Gtk.DialogFlags.DestroyWithParent);
			dialog.Modal = true;
			dialog.Resizable = false;
			Gtk.Label etiqueta = new Gtk.Label();
			etiqueta.Markup = "Se ha cargado con exito.";
			dialog.BorderWidth = 8;
			dialog.VBox.BorderWidth = 8;
			dialog.VBox.PackStart(etiqueta, false, false, 0);
			dialog.AddButton ("Cerrar", ResponseType.Close);
			dialog.ShowAll();
			
			dialog.Run ();
			dialog.Destroy ();
			
			
		}

		public void CargarProductos ()
		{

			productoventa= this.db.ObtenerProductosVenta(Int32.Parse(entry1.Text.Trim()));
			treeview2.Model = this.ventamodel;
			foreach (Produc bod in this.productoventa)
			{
				ventamodel.AppendValues(1,bod.Nombre, bod.Precio);
				preciototal=preciototal+Int32.Parse(bod.Precio);
				Console.WriteLine(preciototal);
				label6.Text=preciototal.ToString();
			}				
			entry1.DeleteText(0, entry1.Text.Length);


			this.treeview2.Selection.UnselectAll();
			
	
		}



		protected void TreeView2SelectionChanged (object sender, EventArgs args)
		{	
			Gtk.TreeIter iter;
			TreeModel model;
			if (this.treeview2.Selection.GetSelected(out model, out iter))
			{
				//model.SetValue(iter, 0, 1);
				//model.SetValue(iter, 1, "New Value");

				//this.entry1.Text = this.ventamodel.GetValue(iter, 0).ToString();

			}
			else
			{
			}
		}
		//The event-invoking method that derived classes can override.
		protected virtual void OnEdicionDialogChanged(EdicionDialogChangedEventArgs e)
		{
			// Make a temporary copy of the event to avoid possibility of
			// a race condition if the last subscriber unsubscribes
			// immediately after the null check and before the event is raised.
			EventHandler<EdicionDialogChangedEventArgs> handler = EdicionDialogChanged;
			if (handler != null)
			{

				handler(this, e);
			}
		}
		public void  Run ()
		{
			base.Run();
			
		}
		protected void OnButton81Clicked (object sender, EventArgs e)
		{
			DetalleVenta pago = new DetalleVenta(Int32.Parse(entry2.Text.Trim()+1),1,Int32.Parse(label6.Text.Trim()),DateTime.Now);
			Console.WriteLine(DateTime.Now);
			this.db.AgregarVentaDetalle(pago);

			Pagar rcd = new Pagar(this);
			try 
			{
				rcd.Run();
				rcd.Destroy();
			}
			catch (MySql.Data.MySqlClient.MySqlException ex)
			{
				rcd.Destroy();
#if DEBUG
				
				
				Console.WriteLine(ex.Message);
#endif
			}		


		
					}

		protected void OnEntry1TextInserted (object o, TextInsertedArgs args)
		{
			CargarProductos();
			cambiado=true;

		}

		protected void OnButton85Clicked (object sender, EventArgs e)
		{
			entry2.IsEditable=true;
			}

		protected void OnEntry2TextInserted (object o, TextInsertedArgs args)
		{
			//que hace si apreta guardar boleta
			boleta=entry2.Text;
		}
		protected void OnEntry1EditingDone (object sender, EventArgs e)
		{
//ventana verder
		}
	}
}

		
		