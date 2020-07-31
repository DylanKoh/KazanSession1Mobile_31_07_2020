using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KazanSession1Mobile_31_07_2020
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisteringAndEditingAsset : ContentPage
    {
        int _assetID = 0;
        public RegisteringAndEditingAsset(int AssetID)
        {
            InitializeComponent();
            _assetID = AssetID;
        }

        private void pDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pAssetGroup_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}