using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;


namespace LuisRuiz
{

	[ToolboxBitmap(typeof(DataPropertyGrid), "DataPropertyGrid.bmp")]
	public class DataPropertyGrid : System.Windows.Forms.PropertyGrid
	{
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataPropertyGrid()
		{
			// Llamada necesaria para el Diseñador de formularios Windows.Forms.
			InitializeComponent();

			// TODO: agregar cualquier inicialización después de llamar a InitComponent

		}

		/// <summary>
		/// Limpiar los recursos que se estén utilizando.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Código generado por el Diseñador de componentes
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		#region CONSTRUCTORES
		
		#endregion
		
		#region ENUMERACIONES
		
		#endregion
		
		#region CAMPOS

		private System.Collections.Hashtable ColCampos = new System.Collections.Hashtable();
		private System.Collections.Hashtable ColNombreCampos = new System.Collections.Hashtable();
		private System.Collections.Hashtable ColCategorias = new System.Collections.Hashtable();
		private System.Collections.Hashtable ColDescripciones = new System.Collections.Hashtable();
		private System.Collections.Hashtable ColOrden = new System.Collections.Hashtable();
		private System.Collections.Hashtable ColReadOnly = new System.Collections.Hashtable();

		private object Objeto = null;
		private System.Data.DataSet DS = new System.Data.DataSet();
		private System.Data.DataTable Tabla = new System.Data.DataTable();
		private System.Data.DataRow Registro = null;
		
		#endregion
		
		#region PROPIEDADES
		
		#endregion
		
		#region METODOS

		public void MuestraDatos(System.Data.DataRow row)
		{
			this.Registro = row;
			if(row.Table != null)
			{
				this.Tabla = row.Table;
				if(row.Table.DataSet != null){this.DS = row.Table.DataSet;}
			}
			this.CreaObjeto(this.CreaCodigoSimple());
			this.SelectedObject = this.Objeto;
		}

		public void MuestraDatos(System.Data.DataRow row,string campos,string nombreCampos)
		{
			this.Registro = row;
			if(row.Table != null)
			{
				this.Tabla = row.Table;
				if(row.Table.DataSet != null){this.DS = row.Table.DataSet;}
			}
			string[] Campos = campos.Split((char)","[0]);
			string[] NombreCampos = nombreCampos.Split((char)","[0]);
			for(int i = 0; i < Campos.Length;i++)
			{
				ColCampos.Add(Campos[i],Campos[i]);
			}
			for(int i = 0; i < NombreCampos.Length;i++)
			{
				ColNombreCampos.Add(Campos[i],NombreCampos[i]);
			}
			this.CreaObjeto(this.CreaCodigoCompleto());
			this.SelectedObject = this.Objeto;
		}

		public void MuestraDatos(System.Data.DataRow row,string campos,string nombreCampos,string categorias,string descripciones,string orden,string readOnly)
		{
			this.Registro = row;
			if(row.Table != null)
			{
				this.Tabla = row.Table;
				if(row.Table.DataSet != null){this.DS = row.Table.DataSet;}
			}
			string[] Campos = campos.Split((char)","[0]);
			string[] NombreCampos = nombreCampos.Split((char)","[0]);
			string[] Categorias = categorias.Split((char)","[0]);
			string[] Descripciones = descripciones.Split((char)","[0]);
			string[] Orden =  orden.Split((char)","[0]);
			string[] ReadOnly =  readOnly.Split((char)","[0]);
			for(int i = 0; i < Campos.Length;i++)
			{
				ColCampos.Add(Campos[i],Campos[i]);
				if(ReadOnly[i] == "1")
				{ColReadOnly.Add(Campos[i],true);}
				else
				{ColReadOnly.Add(Campos[i],false);}
			}
			for(int i = 0; i < NombreCampos.Length;i++)
			{
				ColNombreCampos.Add(Campos[i],NombreCampos[i]);
			}
			for(int i = 0; i < Categorias.Length;i++)
			{
				ColCategorias.Add(Campos[i],Categorias[i]);
			}
			for(int i = 0; i < Descripciones.Length;i++)
			{
				ColDescripciones.Add(Campos[i],Descripciones[i]);
			}
			for(int i = 0; i < Orden.Length;i++)
			{
				ColOrden.Add(Campos[i],Orden[i]);
			}
			this.CreaObjeto(this.CreaCodigoCompleto());
			this.SelectedObject = this.Objeto;
		}

		public void MuestraDatos(System.Data.DataSet dataSet)
		{
		}

		private string CreaCodigoSimple()
		{
			System.Collections.SortedList ColRelaciones = new System.Collections.SortedList();
			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					ColRelaciones.Add(tabla.Columns[0].ColumnName,tabla.Columns[1].ColumnName);
				}
			}
			string codigo = @"
using System;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace miNamespace {
public class miClase {
			
			private System.Data.DataRow m_Row = null;
";
			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					codigo += "public System.Collections.Hashtable Col"+ tabla.Columns[0].ColumnName + " = new System.Collections.Hashtable();\n";				
					codigo += "public System.Collections.Hashtable _Col"+ tabla.Columns[0].ColumnName + " = new System.Collections.Hashtable();\n";
				}
			}
			
			codigo += @"public miClase(System.Data.DataRow row)
	{
		m_Row = row;
";

			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					codigo += "foreach(System.Data.DataRow fila in row.Table.DataSet.Tables[\"" + tabla.TableName + "\"].Rows)";
					codigo += @"{
";
					codigo += "Col" + tabla.Columns[0].ColumnName + ".Add(Col" + tabla.Columns[0].ColumnName + ".Count,fila[0]);\n";
					codigo += "_Col" + tabla.Columns[0].ColumnName + ".Add(fila[0],_Col" + tabla.Columns[0].ColumnName + ".Count);\n";
					codigo += @"}
";
				}
			}  

			codigo += "\n";
			foreach(System.Data.DataColumn col in this.Tabla.Columns)
			{
				if(ColRelaciones.ContainsKey(col.ColumnName))
				{

					codigo += "\t\tif(row[\"" + col.ColumnName + "\"] != DBNull.Value)\n";
					codigo += "\t\t{\n";
					codigo += "\t\tif(Col" + col.ColumnName + "[row[\"" + col.ColumnName + "\"]] != null)\n";
					codigo += "\t\t{\n";
					codigo += "\t\t\tthis." + ColRelaciones[col.ColumnName].ToString() + " = ("+ col.ColumnName + "_Enum)Col" + col.ColumnName + "[row[\"" + col.ColumnName + "\"]];\n";
					codigo += "\t\t}\n";
					codigo += "\t\t}\n";
				}
				else
				{
					codigo += "\t\tif(row[\"" + col.ColumnName + "\"] != DBNull.Value)\n";
					codigo += "\t\t{\n";
					codigo += "\t\t\tthis." + col.ColumnName + " = (" + col.DataType.UnderlyingSystemType.ToString() + ")row[\"" + col.ColumnName + "\"];\n";
					codigo += "\t\t}\n";
				}
			}
			codigo += "}\n";
			foreach(System.Data.DataColumn col in this.Tabla.Columns)
			{
				if(ColRelaciones.ContainsKey(col.ColumnName))
				{
					string campo = col.ColumnName + "_Enum m_" + ColRelaciones[col.ColumnName].ToString();
					string propiedad = col.ColumnName + "_Enum " + ColRelaciones[col.ColumnName].ToString();
					codigo +=  "private " + campo  + ";\n";
					codigo +=  "public " + propiedad + @"
	{
		get { return m_" + ColRelaciones[col.ColumnName].ToString() + @"; }
		set { 
				m_" + ColRelaciones[col.ColumnName].ToString() + @" = value;";
					codigo += "m_Row[\"" + col.ColumnName + "\"] = _Col" + col.ColumnName + "[(int)value];";
					codigo += @"
			}
	}

	";
				}
				else
				{
					string campo = col.DataType.UnderlyingSystemType.ToString() + " m_" + col.ColumnName;
					string propiedad = col.DataType.UnderlyingSystemType.ToString() + " " + col.ColumnName;
					codigo +=  "private " + campo  + ";\n";
					codigo += @"
	public " + propiedad + @"
	{
		get { return m_" + col.ColumnName + @"; }
		set { 
				m_" + col.ColumnName + @" = value;";
					codigo += "m_Row[\"" + col.ColumnName + "\"] = value;";
					codigo += @"
			}
	}

	";
				}
			}
			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					codigo +=  @"

		public enum "; codigo += tabla.Columns[0].ColumnName + @"_Enum
		{";
					int i = 0;
					foreach(System.Data.DataRow fila in tabla.Rows)
					{
						codigo += ObtieneEnumeracion(fila[1].ToString()) + " = " + i + ",\n";
						i ++;
					}
					codigo += "}";

				}
			}
			codigo +="}}";

			return codigo;
		}

		private string CreaCodigoCompleto()
		{
			System.Collections.SortedList ColRelaciones = new System.Collections.SortedList();
			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					ColRelaciones.Add(tabla.Columns[0].ColumnName,tabla.Columns[1].ColumnName);
				}
			}
			string codigo = @"
using System;
using System.Data;
using System.ComponentModel;
using System.Reflection;

namespace miNamespace {
[TypeConverter(typeof(PropertySorter))]
public class miClase {
			
			private System.Data.DataRow m_Row = null;
";
			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					codigo += "public System.Collections.Hashtable Col"+ tabla.Columns[0].ColumnName + " = new System.Collections.Hashtable();\n";				
					codigo += "public System.Collections.Hashtable _Col"+ tabla.Columns[0].ColumnName + " = new System.Collections.Hashtable();\n";
				}
			}
			
			codigo += @"public miClase(System.Data.DataRow row)
	{
		m_Row = row;
";

			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					codigo += "foreach(System.Data.DataRow fila in row.Table.DataSet.Tables[\"" + tabla.TableName + "\"].Rows)";
					codigo += @"{
";
					codigo += "Col" + tabla.Columns[0].ColumnName + ".Add(Col" + tabla.Columns[0].ColumnName + ".Count,fila[0]);\n";
					codigo += "_Col" + tabla.Columns[0].ColumnName + ".Add(fila[0],_Col" + tabla.Columns[0].ColumnName + ".Count);\n";
					codigo += @"}
";
				}
			}  

			codigo += "\n";
			foreach(System.Data.DataColumn col in this.Tabla.Columns)
			{
				if(this.ColCampos.ContainsKey(col.ColumnName))
				{
					if(ColRelaciones.ContainsKey(col.ColumnName))
					{

						codigo += "\t\tif(row[\"" + col.ColumnName + "\"] != DBNull.Value)\n";
						codigo += "\t\t{\n";
						codigo += "\t\tif(_Col" + col.ColumnName + "[row[\"" + col.ColumnName + "\"]] != null)\n";
						codigo += "\t\t{\n";
						codigo += "\t\t\tthis." + this.ColNombreCampos[col.ColumnName].ToString() + " = ("+ col.ColumnName + "_Enum)_Col" + col.ColumnName + "[row[\"" + col.ColumnName + "\"]];\n";
						codigo += "\t\t}\n";
						codigo += "\t\t}\n";
					}
					else
					{
						codigo += "\t\tif(row[\"" + col.ColumnName + "\"] != DBNull.Value)\n";
						codigo += "\t\t{\n";
						codigo += "\t\t\tthis." + this.ColNombreCampos[col.ColumnName].ToString() + " = (" + col.DataType.UnderlyingSystemType.ToString() + ")row[\"" + col.ColumnName + "\"];\n";
						codigo += "\t\t}\n";
					}
				}
			}
			codigo += "}\n";
			foreach(System.Data.DataColumn col in this.Tabla.Columns)
			{
				if(this.ColCampos.ContainsKey(col.ColumnName))
				{
					if(ColRelaciones.ContainsKey(col.ColumnName))
					{
						string campo = col.ColumnName + "_Enum m_" + ColNombreCampos[col.ColumnName].ToString();
						string propiedad = col.ColumnName + "_Enum " + ColNombreCampos[col.ColumnName].ToString();
						codigo +=  "private " + campo  + ";\n";
						codigo += "[Category(\"" + ColCategorias[col.ColumnName].ToString() 
							+ "\"),Description(\"" + ColDescripciones[col.ColumnName].ToString()
							+ "\"),PropertyOrder(" + ColOrden[col.ColumnName].ToString() 
							+ "),ReadOnly(" + ColReadOnly[col.ColumnName].ToString().ToLower() + ")]\n";
						codigo +=  "public " + propiedad + @"
	{
		get { return m_" + ColNombreCampos[col.ColumnName].ToString() + @"; }
		set { 
				m_" + ColNombreCampos[col.ColumnName].ToString() + @" = ("+ col.ColumnName + "_Enum)value;";
						codigo += "m_Row[\"" + col.ColumnName + "\"] = Col" + col.ColumnName + "[(int)value];";
						codigo += @"
			}
	}

	";
					}
					else
					{
						string campo = col.DataType.UnderlyingSystemType.ToString() + " m_" + ColNombreCampos[col.ColumnName].ToString();
						string propiedad = col.DataType.UnderlyingSystemType.ToString() + " " + ColNombreCampos[col.ColumnName].ToString();
						codigo +=  "private " + campo  + ";\n";
						codigo += "[Category(\"" + ColCategorias[col.ColumnName].ToString() 
							+ "\"),Description(\"" + ColDescripciones[col.ColumnName].ToString()
							+ "\"),PropertyOrder(" + ColOrden[col.ColumnName].ToString()
							+ "),ReadOnly(" + ColReadOnly[col.ColumnName].ToString().ToLower() + ")]\n";
						codigo += @"
	public " + propiedad + @"
	{
		get { return m_" + ColNombreCampos[col.ColumnName].ToString() + @"; }
		set { 
				m_" +ColNombreCampos[col.ColumnName].ToString() + @" = value;";
						codigo += "m_Row[\"" + col.ColumnName + "\"] = value;";
						codigo += @"
			}
	}

	";
					}
				}
			}
			foreach(System.Data.DataTable tabla in this.DS.Tables)
			{
				if(tabla != this.DS.Tables[0])
				{
					codigo +=  @"

		public enum "; codigo += tabla.Columns[0].ColumnName + @"_Enum
		{";
					int i = 0;
					foreach(System.Data.DataRow fila in tabla.Rows)
					{
						codigo += ObtieneEnumeracion(fila[1].ToString()) + " = " + i + ",\n";
						i ++;
					}
					codigo += "}";

				}
			}
			codigo +="}}";

			return codigo;
		}

		private string ObtieneEnumeracion(string valor)
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\W");
			valor = valor.Replace(" ","_");
			valor = regex.Replace(valor,"");
			return valor;
		}

		private void CreaObjeto(string codigo)
		{
			ICodeCompiler compilador = new CSharpCodeProvider().CreateCompiler();
			CompilerParameters parametros = new CompilerParameters();

			parametros.ReferencedAssemblies.Add("System.dll");
			parametros.ReferencedAssemblies.Add("System.Data.dll");
			parametros.ReferencedAssemblies.Add("System.Xml.dll");
			parametros.ReferencedAssemblies.Add(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll");
			parametros.GenerateInMemory = true;
			
			CompilerResults compilado = compilador.CompileAssemblyFromSource(parametros,codigo);

			if (compilado.Errors.HasErrors) 
			{
				string mensaje = "";

				mensaje = compilado.Errors.Count.ToString() + " Errores:";
				for (int x=0;x<compilado.Errors.Count;x++) 
					mensaje = mensaje  + "\r\nLine: " + compilado.Errors[x].Line.ToString() + " - " + 
						compilado.Errors[x].ErrorText;		

								frmCodigo v = new frmCodigo();
								v.textBox1.Text = mensaje + "\r\n\r\n" + codigo;
								v.ShowDialog();
//				MessageBox.Show(mensaje + "\r\n\r\n" + codigo,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);

				return;
			}

			Assembly ensambaldo = compilado.CompiledAssembly;

			this.Objeto = ensambaldo.CreateInstance("miNamespace.miClase",false,BindingFlags.CreateInstance,null,new object[]{this.Registro},System.Globalization.CultureInfo.CurrentCulture,null);
			if (this.Objeto == null) 
			{
				MessageBox.Show("No se pudo cargar la clase!.");
				return;
			}
		}
		
		
		#endregion
		
		#region EVENTOS
		
		#endregion
	}

}

//
// (C) Paul Tingey 2004 
//

public class PropertySorter : ExpandableObjectConverter
{
	#region Methods
	public override bool GetPropertiesSupported(ITypeDescriptorContext context) 
	{
		return true;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
	{
		//
		// This override returns a list of properties in order
		//
		PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(value, attributes);
		ArrayList orderedProperties = new ArrayList();
		foreach (PropertyDescriptor pd in pdc)
		{
			Attribute attribute = pd.Attributes[typeof(PropertyOrderAttribute)];
			if (attribute != null)
			{
				//
				// If the attribute is found, then create an pair object to hold it
				//
				PropertyOrderAttribute poa = (PropertyOrderAttribute)attribute;
				orderedProperties.Add(new PropertyOrderPair(pd.Name,poa.Order));
			}
			else
			{
				//
				// If no order attribute is specifed then given it an order of 0
				//
				orderedProperties.Add(new PropertyOrderPair(pd.Name,0));
			}
		}
		//
		// Perform the actual order using the value PropertyOrderPair classes
		// implementation of IComparable to sort
		//
		orderedProperties.Sort();
		//
		// Build a string list of the ordered names
		//
		ArrayList propertyNames = new ArrayList();
		foreach (PropertyOrderPair pop in orderedProperties)
		{
			propertyNames.Add(pop.Name);
		}
		//
		// Pass in the ordered list for the PropertyDescriptorCollection to sort by
		//
		return pdc.Sort((string[])propertyNames.ToArray(typeof(string)));
	}
	#endregion
}

#region Helper Class - PropertyOrderAttribute
[AttributeUsage(AttributeTargets.Property)]
public class PropertyOrderAttribute : Attribute
{
	//
	// Simple attribute to allow the order of a property to be specified
	//
	private int _order;
	public PropertyOrderAttribute(int order)
	{
		_order = order;
	}

	public int Order
	{
		get
		{
			return _order;
		}
	}
}
#endregion

#region Helper Class - PropertyOrderPair
public class PropertyOrderPair : IComparable
{
	private int _order;
	private string _name;
	public string Name
	{
		get
		{
			return _name;
		}
	}

	public PropertyOrderPair(string name, int order)
	{
		_order = order;
		_name = name;
	}

	public int CompareTo(object obj)
	{
		//
		// Sort the pair objects by ordering by order value
		// Equal values get the same rank
		//
		int otherOrder = ((PropertyOrderPair)obj)._order;
		if (otherOrder == _order)
		{
			//
			// If order not specified, sort by name
			//
			string otherName = ((PropertyOrderPair)obj)._name;
			return string.Compare(_name,otherName);
		}
		else if (otherOrder > _order)
		{
			return -1;
		}
		return 1;
	}
}
#endregion
