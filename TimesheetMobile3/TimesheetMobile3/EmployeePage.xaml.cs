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

namespace TimesheetMobile3
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmployeePage : ContentPage
	{
		public EmployeePage ()
		{
			InitializeComponent ();

            employeeList.ItemsSource = new string[] { "" };
            employeeList.ItemSelected += EmployeeList_ItemSelected;


        }
        private async void OttaaFoto(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                await DisplayAlert("Ops", "Nenhuma câmera detectada.", "OK");

                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Demo"
                });

            if (file == null)
                return;

            MinhaImagem.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;

            });
        }

        private async void EmployeeList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            string employee = employeeList.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(employee))
            {

                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("http://k2-timesheetmobile.azurewebsites.net");

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
                client.BaseAddress = new Uri("http://k2-timesheetmobile.azurewebsites.net");
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