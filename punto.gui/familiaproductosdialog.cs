using System;
using System.Collections.Generic;
using Gtk;
using punto.code;
namespace punto.gui
{
	public partial class familiaproductosdialog : Gtk.Dialog
	{

		private ControladorBaseDatos db;
		
		public List<FamiliaProducto> familias = new List<FamiliaProducto>();
		
		private Gtk.ListStore bodegasmodel;
		

		public event EventHandler<EdicionDialogChangedEventArgs> EdicionDialogChanged;
		private bool cambiado = false;
		
		public familiaproductosdialog (Gtk.Window parent) : base ("Administrar familia", parent, Gtk.DialogFlags.DestroyWithParent)
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
			
		
			
			Gtk.TreeViewColumn nombre_column = new Gtk.TreeViewColumn();
			nombre_column.Title = "Nombre";
			Gtk.CellRendererText nombre_cell = new Gtk.CellRendererText();
			nombre_column.PackStart(nombre_cell, true);
			this.familiaplantastreeview.AppendColumn(nombre_column);
			nombre_column.AddAttribute(nombre_cell, "text", 0);
			
			this.familiaplantastreeview.Selection.Changed += OnFamiliasTreeViewSelectionChanged;

			
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
		public void  Run () 
		{
			this.CargarFamilias();
			base.Run();
			
		}


		public void CargarFamilias()
		{
			this.familias = this.db.ObtenerFamiliasBd();
			this.bodegasmodel = new Gtk.ListStore(typeof(string));
			foreach (FamiliaProducto prod in this.familias)
			{
				this.bodegasmodel.AppendValues( prod.Nombre);
			}
			familiaplantastreeview.Model = this.bodegasmodel;
			

		}
		protected void OnFamiliasTreeViewSelectionChanged (object sender, EventArgs args)
		{	
			Gtk.TreeIter iter;
			if (this.familiaplantastreeview.Selection.GetSelected(out iter))
			{
				agregar_button.Sensitive = true;

				this.actualizar_button.Sensitive = true;
			}
			else
			{
				this.actualizar_button.Sensitive = false;
			}
		}

		protected virtual void OnAgregarButtonClicked (object sender, System.EventArgs e)
		{
			FamiliaProducto prod = new FamiliaProducto(this.entry.Text.Trim());


			if (this.db.ExisteFamiliaBd(prod))
			{
				Dialog dialog = new Dialog("FAMILIA YA EXISTE", this, Gtk.DialogFlags.DestroyWithParent);
				dialog.Modal = true;
				dialog.Resizable = false;
				Gtk.Label etiqueta = new Gtk.Label();
				etiqueta.Markup = "La Familia que intenta agregar ya existe en la Base de Datos";
				dialog.BorderWidth = 8;
				dialog.VBox.BorderWidth = 8;
				dialog.VBox.PackStart(etiqueta, false, false, 0);
				dialog.AddButton ("Cerrar", ResponseType.Close);
				dialog.ShowAll();		
			    dialog.Run ();
				dialog.Destroy ();
			}
			else
			{
				if (this.db.AgregarFamiliaBd(prod))
				{
					this.familias.Add(prod);
					this.bodegasmodel.AppendValues(prod.Nombre);
					
					this.entry.Text = "";
					this.familiaplantastreeview.Selection.UnselectAll();
					this.agregar_button.Sensitive = false;
				
					agregar_button.Sensitive = true;

					this.cambiado = true;
				}
				else
				{
					Dialog dialog = new Dialog("ERROR AL AGREGAR FAMILIA", this, Gtk.DialogFlags.DestroyWithParent);
					dialog.Modal = true;
					dialog.Resizable = false;
					Gtk.Label etiqueta = new Gtk.Label();
					etiqueta.Markup = "Ha ocurrido un error al agregar la Familia a la Base de Datos";
					dialog.BorderWidth = 8;
					dialog.VBox.BorderWidth = 8;
					dialog.VBox.PackStart(etiqueta, false, false, 0);
					dialog.AddButton ("Cerrar", ResponseType.Close);
					dialog.ShowAll();
					dialog.Run ();
					dialog.Destroy ();
					agregar_button.Sensitive = true;

				}
			}

		}

		protected void OnActualizarButtonClicked (object sender, EventArgs e)
		{
			throw new System.NotImplementedException ();
		}
}
}

