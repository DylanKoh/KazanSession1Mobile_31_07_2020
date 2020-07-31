using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static KazanSession1Mobile_31_07_2020.GlobalGlass;

namespace KazanSession1Mobile_31_07_2020
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        List<Department> _departments;
        List<AssetGroup> _assetGroups;
        List<GetCustomViews> _originalSource;
        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await LoadPickers();
            await LoadData();
        }


        private async Task LoadData()
        {
            var client = new WebApi();
            var getCustomView = await client.PostAsync("Assets/GetCustomViews", null);
            _originalSource = JsonConvert.DeserializeObject<List<GetCustomViews>>(getCustomView);
            lvAssets.ItemsSource = _originalSource;
        }

        private async Task LoadPickers()
        {
            pAssetGroup.Items.Clear();
            pDepartment.Items.Clear();

            pAssetGroup.Items.Add("No Filter");
            pDepartment.Items.Add("No Filter");
            var client = new WebApi();
            var getAssetGroup = await client.PostAsync("AssetGroups", null);
            _assetGroups = JsonConvert.DeserializeObject<List<AssetGroup>>(getAssetGroup);
            foreach (var item in _assetGroups)
            {
                pAssetGroup.Items.Add(item.Name);
            }

            var getDepartments = await client.PostAsync("Departments", null);
            _departments = JsonConvert.DeserializeObject<List<Department>>(getDepartments);
            foreach (var item in _departments)
            {
                pDepartment.Items.Add(item.Name);
            }
        }

        private void FilterContent()
        {
            if ((pDepartment.SelectedItem == null && pAssetGroup.SelectedItem == null) || (pDepartment.SelectedItem.ToString() == "No Filter" && pAssetGroup.SelectedItem.ToString() == "No Filter"))
            {
                if (string.IsNullOrWhiteSpace(sbSearch.Text))
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
                else
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           where x.AssetName.ToLower().Contains(sbSearch.Text.ToLower()) || x.AssetSN.Contains(sbSearch.Text)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
            }
            else if (pDepartment.SelectedItem == null || pDepartment.SelectedItem.ToString() == "No Filter")
            {
                if (string.IsNullOrWhiteSpace(sbSearch.Text))
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.AssetGroup == pAssetGroup.SelectedItem.ToString()
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
                else
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           where x.AssetGroup == pAssetGroup.SelectedItem.ToString()
                                           where x.AssetName.ToLower().Contains(sbSearch.Text.ToLower()) || x.AssetSN.Contains(sbSearch.Text)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
            }
            else if (pAssetGroup.SelectedItem == null || pAssetGroup.SelectedItem.ToString() == "No Filter")
            {
                if (string.IsNullOrWhiteSpace(sbSearch.Text))
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Department == pDepartment.SelectedItem.ToString()
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;

                }
                else
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           where x.Department == pDepartment.SelectedItem.ToString()
                                           where x.AssetName.ToLower().Contains(sbSearch.Text.ToLower()) || x.AssetSN.Contains(sbSearch.Text)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(sbSearch.Text))
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Department == pDepartment.SelectedItem.ToString() && x.AssetGroup == pAssetGroup.SelectedItem.ToString()
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
                else
                {
                    var getFilteredList = (from x in _originalSource
                                           where x.Warranty == null || (x.Warranty > dpStart.Date && x.Warranty < dpEnd.Date)
                                           where x.Department == pDepartment.SelectedItem.ToString() && x.AssetGroup == pAssetGroup.SelectedItem.ToString()
                                           where x.AssetName.ToLower().Contains(sbSearch.Text.ToLower()) || x.AssetSN.Contains(sbSearch.Text)
                                           select x).ToList();
                    lvAssets.ItemsSource = getFilteredList;
                }
            }
        }

        private void pDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
           FilterContent();
        }



        private void pAssetGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterContent();
        }

        private void dpStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            FilterContent();
        }

        private void dpEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            FilterContent();
        }

        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            var trigger = (ImageButton)sender;
            var getParentLayout = (StackLayout)trigger.Parent;
            var getMainParent = (Grid)getParentLayout.Parent;
            var getChildLayout = (StackLayout)getMainParent.Children[0];
            var getAssetID = Int32.Parse(((Label)getChildLayout.Children[0]).Text);

        }

        private void btnMove_Clicked(object sender, EventArgs e)
        {
            var trigger = (ImageButton)sender;
            var getParentLayout = (StackLayout)trigger.Parent;
            var getMainParent = (Grid)getParentLayout.Parent;
            var getChildLayout = (StackLayout)getMainParent.Children[0];
            var getAssetID = Int32.Parse(((Label)getChildLayout.Children[0]).Text);
        }

        private void btnHistory_Clicked(object sender, EventArgs e)
        {
            var trigger = (ImageButton)sender;
            var getParentLayout = (StackLayout)trigger.Parent;
            var getMainParent = (Grid)getParentLayout.Parent;
            var getChildLayout = (StackLayout)getMainParent.Children[0];
            var getAssetID = Int32.Parse(((Label)getChildLayout.Children[0]).Text);
        }

        private void sbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterContent();
        }
    }
}
