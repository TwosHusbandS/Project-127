using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Project_127.HelperClasses
{
	class DataGridHelper
	{
		public static void SelectFirst(DataGrid myDG)
		{
			if (myDG.Items.Count > 0)
			{
				SelectRowByIndex(myDG, 0);
			}
		}

		public static void SelectRowByIndex(DataGrid dataGrid, int rowIndex)
		{
			// AAA - THIS IS ALSO USED BY SAVEFILE HANDLER. JUST...be aware of it XD

			if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
				//throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");

				if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
					//throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

					dataGrid.SelectedItems.Clear();
			/* set the SelectedItem property */
			object item = dataGrid.Items[rowIndex]; // = Product X
			dataGrid.SelectedItem = item;

			DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
			if (row == null)
			{
				/* bring the data item (Product object) into view
				 * in case it has been virtualized away */
				dataGrid.ScrollIntoView(item);
				row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
			}
			if (row != null)
			{
				DataGridCell cell = GetCell(dataGrid, row, 0);
				if (cell != null)
					cell.Focus();
			}
		}

		public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
					return (T)child;
				else
				{
					T childOfChild = FindVisualChild<T>(child);
					if (childOfChild != null)
						return childOfChild;
				}
			}
			return null;
		}

		public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
		{
			if (rowContainer != null)
			{
				DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				if (presenter == null)
				{
					/* if the row has been virtualized away, call its ApplyTemplate() method 
					 * to build its visual tree in order for the DataGridCellsPresenter
					 * and the DataGridCells to be created */
					rowContainer.ApplyTemplate();
					presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
				}
				if (presenter != null)
				{
					DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					if (cell == null)
					{
						/* bring the column into view
						 * in case it has been virtualized away */
						dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
						cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
					}
					return cell;
				}
			}
			return null;
		}
	}
}
