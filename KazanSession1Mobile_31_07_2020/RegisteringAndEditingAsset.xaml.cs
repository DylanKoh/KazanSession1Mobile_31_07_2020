using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static KazanSession1Mobile_31_07_2020.GlobalGlass;

namespace KazanSession1Mobile_31_07_2020
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisteringAndEditingAsset : ContentPage
    {
        int _assetID = 0;
        List<Department> _departments;
        List<AssetGroup> _assetGroups;
        Asset _asset;
        List<Location> _locations;
        List<DepartmentLocation> _departmentLocations;
        List<Employee> _employees;
        public RegisteringAndEditingAsset(int AssetID)
        {
            InitializeComponent();
            _assetID = AssetID;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await LoadPickers();
            if (_assetID != 0)
            {
                await LoadAsset();
            }
        }

        private async Task LoadAsset()
        {
            var client = new WebApi();

            var getCurrentAsset = await client.PostAsync($"Assets/Details/{_assetID}", null);
            _asset = JsonConvert.DeserializeObject<Asset>(getCurrentAsset);
            entryAsset.Text = _asset.AssetName;
            pDepartment.SelectedItem = (from x in _departmentLocations
                                        join y in _departments on x.DepartmentID equals y.ID
                                        where x.ID == _asset.DepartmentLocationID
                                        select y.Name).First();
            await Task.Delay(500);
            pLocation.SelectedItem = (from x in _departmentLocations
                                      join y in _locations on x.LocationID equals y.ID
                                      where x.ID == _asset.DepartmentLocationID
                                      select y.Name).First();
            pAccountable.SelectedItem = (from x in _employees
                                         where x.ID == _asset.EmployeeID
                                         select x.FirstName + " " + x.LastName).First();
            pAssetGroup.SelectedItem = (from x in _assetGroups
                                        where x.ID == _asset.AssetGroupID
                                        select x.Name).First();
            editorDescription.Text = _asset.Description;
            dpWarranty.Text = _asset.WarrantyDate.ToString();
            lblAssetSN.Text = _asset.AssetSN;

            pLocation.IsEnabled = false;
            pDepartment.IsEnabled = false;
            pAssetGroup.IsEnabled = false;
            entryAsset.IsEnabled = false;
        }

        private async Task LoadPickers()
        {
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

            var getDepartmentLocations = await client.PostAsync("DepartmentLocations", null);
            _departmentLocations = JsonConvert.DeserializeObject<List<DepartmentLocation>>(getDepartmentLocations);

            var getLocations = await client.PostAsync("Locations", null);
            _locations = JsonConvert.DeserializeObject<List<Location>>(getLocations);

            var getAccountable = await client.PostAsync("Employees", null);
            _employees = JsonConvert.DeserializeObject<List<Employee>>(getAccountable);
            foreach (var item in _employees)
            {
                pAccountable.Items.Add(item.FirstName + " " + item.LastName);
            }

        }

        private void pDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLocations();
            pLocation.IsEnabled = true;
            if (pAssetGroup.SelectedItem != null)
            {
                getNewSN();
            }
        }

        private void LoadLocations()
        {
            pLocation.Items.Clear();
            var getDepartmentID = (from x in _departments
                                   where x.Name == pDepartment.SelectedItem.ToString()
                                   select x.ID).First();
            var getLocationsAvailable = (from x in _departmentLocations
                                         join y in _locations on x.LocationID equals y.ID
                                         where x.DepartmentID == getDepartmentID
                                         select y.Name).ToList();
            foreach (var item in getLocationsAvailable)
            {
                pLocation.Items.Add(item);
            }
        }

        private void pAssetGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pDepartment.SelectedItem != null)
            {
                getNewSN();
            }
        }

        private async void btnSubmit_Clicked(object sender, EventArgs e)
        {
            var client = new WebApi();
            if (_assetID == 0)
            {
                var getAssetGroupID = (from x in _assetGroups
                                       where x.Name == pAssetGroup.SelectedItem.ToString()
                                       select x.ID).First();
                var getDepartmentID = (from x in _departments
                                       where x.Name == pDepartment.SelectedItem.ToString()
                                       select x.ID).First();
                var getLocationID = (from x in _locations
                                     where x.Name == pLocation.SelectedItem.ToString()
                                     select x.ID).First();
                var getDepartmentLocationID = (from x in _departmentLocations
                                               where x.DepartmentID == getDepartmentID && x.LocationID == getLocationID
                                               select x.ID).First();
                var getAccountable = (from x in _employees
                                      where pAccountable.SelectedItem.ToString() == x.FirstName + " " + x.LastName
                                      select x.ID).First();
                var newAsset = new Asset()
                {
                    AssetSN = lblAssetSN.Text,
                    AssetName = entryAsset.Text,
                    AssetGroupID = getAssetGroupID,
                    DepartmentLocationID = getDepartmentLocationID,
                    Description = editorDescription.Text,
                    EmployeeID = getAccountable,
                    WarrantyDate = DateTime.Parse(dpWarranty.Text)
                };
                var JsonData = JsonConvert.SerializeObject(newAsset);
                var response = await client.PostAsync("Assets/Create", JsonData);
                if (response == "\"Asset created successfully!\"")
                {
                    await DisplayAlert("Add Asset", "Asset created successfully!", "Ok");
                    await Navigation.PopAsync();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(dpWarranty.Text))
                {
                    _asset.WarrantyDate = null;
                    _asset.EmployeeID = (from x in _employees
                                         where pAccountable.SelectedItem.ToString() == x.FirstName + " " + x.LastName
                                         select x.ID).First();
                    if (string.IsNullOrEmpty(editorDescription.Text) || string.IsNullOrWhiteSpace(editorDescription.Text))
                    {
                        _asset.Description = " ";
                    }
                    else
                    {
                        _asset.Description = editorDescription.Text;
                    }
                }
                else
                {
                    _asset.WarrantyDate = DateTime.Parse(dpWarranty.Text);
                    _asset.EmployeeID = (from x in _employees
                                         where pAccountable.SelectedItem.ToString() == x.FirstName + " " + x.LastName
                                         select x.ID).First();
                    if (string.IsNullOrEmpty(editorDescription.Text) || string.IsNullOrWhiteSpace(editorDescription.Text))
                    {
                        _asset.Description = " ";
                    }
                    else
                    {
                        _asset.Description = editorDescription.Text;
                    }

                }
                var JsonData = JsonConvert.SerializeObject(_asset);
                var response = await client.PostAsync("Assets/Edit", JsonData);
                if (response == "\"Asset edited successfully!\"")
                {
                    await DisplayAlert("Edit Asset", "Asset edited successfully!", "Ok");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Edit Asset", "Something happened! Please try again later!", "Ok");
                }
            }
        }

        private async void btnCancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void dpWarranty_Focused(object sender, FocusEventArgs e)
        {
            dpSelection.Focus();
            dpSelection.DateSelected += DpSelection_DateSelected;
        }

        private void DpSelection_DateSelected(object sender, DateChangedEventArgs e)
        {
            dpWarranty.Text = dpSelection.Date.ToShortDateString();
        }

        private async void getNewSN()
        {
            var client = new WebApi();
            var getUniqueSNs = await client.PostAsync("Assets/GetUniqueSN", null);
            var listOfSN = JsonConvert.DeserializeObject<List<string>>(getUniqueSNs);
            var getAssetGroupID = (from x in _assetGroups
                                   where x.Name == pAssetGroup.SelectedItem.ToString()
                                   select x.ID).First();
            var getDepartmentID = (from x in _departments
                                   where x.Name == pDepartment.SelectedItem.ToString()
                                   select x.ID).First();
            var dd = getDepartmentID.ToString().PadLeft(2, '0');
            var gg = getAssetGroupID.ToString().PadLeft(2, '0');
            var getLatestSN = (from x in listOfSN
                               where x.Contains($"{dd}/{gg}")
                               orderby x descending
                               select x).FirstOrDefault();
            if (getLatestSN == null)
            {
                var newSN = 1.ToString().PadLeft(4, '0');
                lblAssetSN.Text = $"{dd}/{gg}/{newSN}";
            }
            else
            {
                var newSN = (Int32.Parse(getLatestSN.Split('/')[2]) + 1).ToString().PadLeft(4, '0');
                lblAssetSN.Text = $"{dd}/{gg}/{newSN}";
            }

        }
    }
}