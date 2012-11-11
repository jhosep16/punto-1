using System;
using System.Collections.Generic;
using Gtk;
using punto.code;

namespace punto.gui
{
	public partial class VenderProductosDialog : Gtk.Dialog
	{
		private ControladorBaseDatos db;
		
		public List<Produc> productoventa = new List<Produc>();
		private Gtk.ListStore ventamodel;
		
		public event EventHandler<EdicionDialogChangedEventArgs> EdicionDialogChanged;
		private bool cambiado = false;

		public VenderProductosDialog (Gtk.Window parent) : base ("Vender Productos", parent, Gtk.DialogFlags.DestroyWithParent)
		{
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
					Application.Quit();
				}
			}
			Gtk.TreeViewColumn precio_column = new Gtk.TreeViewColumn();
			precio_column.Title = "Precio";
			Gtk.CellRendererText precio_cell = new Gtk.CellRendererText();
			precio_column.PackStart(precio_cell, true);
			
			Gtk.TreeViewColumn nombre_column = new Gtk.TreeViewColumn();
			nombre_column.Title = "Nombre";
			Gtk.CellRendererText nombre_cell = new Gtk.CellRendererText();
			nombre_column.PackStart(nombre_cell, true);

			this.treeview2.AppendColumn(nombre_column);
			nombre_column.AddAttribute(nombre_cell, "text", 0);
			this.treeview2.AppendColumn(precio_column);
			precio_column.AddAttribute(precio_cell, "text", 1);
		

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

			this.productoventa= this.db.ObtenerProductosVenta(Int32.Parse(entry1.Text.Trim()));
			this.ventamodel = new Gtk.ListStore(typeof(string), typeof(int));
			Console.WriteLine("antesdelfor");
			foreach (Produc bod in this.productoventa)
			{
				this.ventamodel.AppendValues(bod.NOMBRES, bod.PRECIO);
				Console.WriteLine(bod.NOMBRES);
				Console.WriteLine("ene elfor");

			}
			this.treeview2.Selection.UnselectAll();

			treeview2.Model = this.ventamodel;
	
		}

		protected void TreeView2SelectionChanged (object sender, EventArgs args)
		{	
			Gtk.TreeIter iter;
			if (this.treeview2.Selection.GetSelected(out iter))
			{
				this.entry1.Text = this.ventamodel.GetValue(iter, 0).ToString();

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


		protected void OnButton81Clicked (object sender, EventArgs e)
		{
			Console.WriteLine("boton");

				this.CargarProductos();
				
					}
	}
}

		
		