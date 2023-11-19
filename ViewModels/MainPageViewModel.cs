using CommunityToolkit.Mvvm.ComponentModel;
using GoingOutMobile.Services;
using GoingOutMobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.ViewModels
{
    public partial class MainPageViewModel : ViewModelGlobal
    {
        [ObservableProperty]
        string nombreUsuario;

        private string UserName;

        //private readonly IGenericQueriesServices _genericQueriesServices;

        //public MainPageViewModel(IGenericQueriesServices genericQueriesServices)
        //{
        //    UserName = Preferences.Get("userName",string.Empty);
        //    _genericQueriesServices = genericQueriesServices;
        //}


        //public async Task LoadDataAsync()
        //{
        //    if (IsBusy)
        //        return;

        //    try
        //    {
        //        IsBusy = true;
        //        var resultado = await _genericQueriesServices.GetInfoUser(UserName);


        //        if (resultado != null)
        //        {
        //            Preferences.Set("Name", resultado.Name);
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        await Application.Current.MainPage.DisplayAlert("Error", e.Message, "Aceptar");
        //    }
        //    finally
        //    {
        //        IsBusy = false;
        //    }


        //}




    }
}
