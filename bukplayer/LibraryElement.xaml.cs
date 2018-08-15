using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BukPlayer
{
    /// <summary>
    /// Interaction logic for LibraryElement.xaml
    /// </summary>
    public abstract partial class LibraryElement : UserControl
    {

        public BitmapImage image;
        public string Title;
        public LibraryElement()
        {
            InitializeComponent();
        }

        public abstract void LoadVisuals();
        public abstract void UnLoadVisuals();
        
    }
}
