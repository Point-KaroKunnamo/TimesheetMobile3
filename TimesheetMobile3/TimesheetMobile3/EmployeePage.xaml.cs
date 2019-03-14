using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using TimesheetMobile3.Models;
using System.IO;
using Android.Content;
using Android.Graphics;
using IOStream = System.IO.Stream;

namespace TimesheetMobile3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeePage : ContentPage
    {
        public Stream stream;
        Plugin.Media.Abstractions.MediaFile mediaFile = null;
        public byte[] imageByteArray;
        //public byte[] memoryStream { get;  set; }
        //public byte[] imageAsByteArray { get;  set; }

        public EmployeePage()
        {
            InitializeComponent();
            

            employeeList.ItemsSource = new string[] { "" };
            employeeList.ItemSelected += EmployeeList_ItemSelected;

            IOStream stream = null;
        }


        public async void TakeFoto(object sender, EventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(employee))
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
                {
                    await DisplayAlert("Ops", "Kameraa ei löydy.", "OK");

                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Demo"//,
                    //SaveMetaData = false
                });

                if (file == null)
                    return;

                
                FotoImage.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    //file.Dispose();
                    return stream;      
                });


                //tallennetaan kuva

                try
                {
                    imageByteArray = System.IO.File.ReadAllBytes(file.Path);
                    
                    FotoModel data = new FotoModel()
                    {
                        Name = employee,
                       Fotodata = imageByteArray
                    };

                    file.Dispose();

                    JsonSerializer _serializer = new JsonSerializer();
                    HttpClient client = new HttpClient();

                    client.BaseAddress = new Uri("https://k2mobilebackend.azurewebsites.net");

                    string input = JsonConvert.SerializeObject(data);
                    StringContent content = new StringContent(input, Encoding.UTF8, "application/json");
                    
                    var httpClient = new HttpClient();
                    HttpResponseMessage message = await client.PostAsync("/api/Employee", content);
                   
                    //string reply = await message.Content.ReadAsStringAsync();
                    //bool success = JsonConvert.DeserializeObject<bool>(reply);
                    //if (success)
                    int statuscode = (int)message.StatusCode;
                    byte[] reply2 = await message.Content.ReadAsByteArrayAsync();
                    if (statuscode >= 200 && statuscode < 300)
                    {
                        await DisplayAlert("save picture", "Picture saved succesfully", "Close");
                        takePictureButton.Text = "Picture Taken!";
                    }

                    else
                    {
                        await DisplayAlert("save picture", "Picture save error.", "Close");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    employeeList.ItemsSource = new string[] { errorMessage };
                }
            }
        }
        private async void EmployeeList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(employee))
            {

                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://k2mobilebackend.azurewebsites.net");

                    string json = await client.GetStringAsync("/api/employee?employeeName=" + employee);
                    byte[] imageBytes = JsonConvert.DeserializeObject<byte[]>(json);

                    employeeImage.Source = ImageSource.FromStream(
                        () => new System.IO.MemoryStream(imageBytes));
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    employeeList.ItemsSource = new string[] { errorMessage };
                }
            }
        }
        //-------------------------------------------------------------

        

        //-------------------------------------------------------------
        public async void LoadEmployees(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://k2mobilebackend.azurewebsites.net");
                string json = await client.GetStringAsync("/api/Employee");
                string[] employees = JsonConvert.DeserializeObject<string[]>(json);

                employeeList.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                employeeList.ItemsSource = new string[] { errorMessage };
            }
        }
        private async void ListWorkAssignments(object sender, EventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(employee))
            {
                await DisplayAlert("List Work", "You must select employee first,", "Ok");
            }
            else
            {
                await Navigation.PushAsync(new WorkAssignmentPage());
            }
        }


       

      

      

   
    }
}