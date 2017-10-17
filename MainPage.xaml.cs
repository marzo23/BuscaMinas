using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BuscaMinas2
{
    public partial class MainPage : ContentPage
    {
        const int _TAMANO_MATRIZ = 8;

        Label [,] btnArray = new Label[_TAMANO_MATRIZ, _TAMANO_MATRIZ];
        int[,] matrizBase = new int[_TAMANO_MATRIZ, _TAMANO_MATRIZ];
        int[,] matrizEdos = new int[_TAMANO_MATRIZ, _TAMANO_MATRIZ];

        public MainPage()
        {
            InitializeComponent();

            inicializaMatrices();
            asignaBombas();

            ///COMPORTAMIENTO BOTONES
            var dosClick = new TapGestureRecognizer();
            dosClick.NumberOfTapsRequired = 2;
            dosClick.Tapped += dobleClick;

            var unClick = new TapGestureRecognizer();
            unClick.Tapped += HandlerBtn;

            //LAYOUT
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                for (int j = 0; j < _TAMANO_MATRIZ; j++)
                {
                    btnArray[i, j] = new Label();
                    btnArray[i, j].VerticalTextAlignment = TextAlignment.Center;
                    btnArray[i, j].HorizontalTextAlignment = TextAlignment.Center;
                    btnArray[i, j].BackgroundColor = Color.FromRgb(60, 99, 100);
                    btnArray[i, j].GestureRecognizers.Add(dosClick);
                    btnArray[i, j].GestureRecognizers.Add(unClick);
                    grid.Children.Add(btnArray[i, j], i, j);
                }
            }

            //OTROS COMPONENTES
            Xamarin.Forms.Button reset = new Xamarin.Forms.Button();
            reset.Text = "Reiniciar Juego";
            reset.Clicked += Reset_Clicked;
            grid.Children.Add(reset, 0, _TAMANO_MATRIZ);
            Grid.SetColumnSpan(reset, _TAMANO_MATRIZ);
        }

        private void dobleClick(object sender, EventArgs e)
        {
            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                for (int j = 0; j < _TAMANO_MATRIZ; j++)
                {
                    if (sender == btnArray[i, j])
                    {
                        cambiaEdo(i, j);
                    }
                }
            }
        }

        private void Reset_Clicked(object sender, EventArgs e)
        {
            inicializaMatrices();
            asignaBombas();

            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                for (int j = 0; j < _TAMANO_MATRIZ; j++)
                {
                    btnArray[i, j].Text = "";
                }
            }

        }

        void inicializaMatrices()
        {
            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                for (int j = 0; j < _TAMANO_MATRIZ; j++)
                {
                    matrizBase[i, j] = 0;
                    matrizEdos[i, j] = 0;
                }
            }
        }

        void asignaBombas()
        {
            int cont = 0;
            while (cont < _TAMANO_MATRIZ)
            {
                Random rand = new Random();
                int x = rand.Next(0, _TAMANO_MATRIZ - 1);
                int y = rand.Next(0, _TAMANO_MATRIZ - 1);
                if (matrizBase[x, y] < 90)
                {
                    matrizBase[x, y] = 90;

                    if (y != _TAMANO_MATRIZ - 1)
                    {
                        matrizBase[x, y + 1]++;

                        if(x!= _TAMANO_MATRIZ-1)
                            matrizBase[x + 1, y + 1]++;
                        if(x!=0)
                            matrizBase[x - 1, y + 1]++;
                    }

                    if (y != 0)
                    {
                        matrizBase[x, y - 1]++;

                        if (x != _TAMANO_MATRIZ - 1)
                            matrizBase[x + 1, y - 1]++;
                        if (x != 0)
                            matrizBase[x - 1, y - 1]++;
                    }

                    if (x != _TAMANO_MATRIZ - 1)
                        matrizBase[x + 1, y]++;

                    if (x != 0)
                        matrizBase[x - 1, y]++;

                    cont++;
                }

            }
        }
            

        private void HandlerBtn(object sender, EventArgs e)
        {
            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                for (int j = 0; j < _TAMANO_MATRIZ; j++)
                {
                    if(sender == btnArray[i, j])
                    {
                        descubrir(i, j);
                    }
                }
            }
        }

        void descubrir(int x, int y)
        {
            if (matrizEdos[x, y] == 0)
            {
                automata(0, x, y);
                btnArray[x, y].IsEnabled = false;
                btnArray[x, y].BackgroundColor = Color.CadetBlue;
                btnArray[x, y].TextColor = Color.Blue;
                btnArray[x, y].Text = "" + matrizBase[x, y];
                if (matrizBase[x, y] > 8)
                    abrirTodo();   
                if (matrizBase[x, y] == 0)
                    abrirCero(x, y);
            }
        }

        void cambiaEdo(int x, int y)
        {
            automata(1, x, y);
            switch (matrizEdos[x, y])
            {
                case 0:
                    btnArray[x, y].Text = "";
                    break;
                case 2:
                    btnArray[x, y].Text = "*";
                    break;
                case 3:
                    btnArray[x, y].Text = "?";
                    break;
            }
        }

        void abrirTodo()
        {
            for (int i = 0; i < _TAMANO_MATRIZ; i++)
            {
                for (int j = 0; j < _TAMANO_MATRIZ; j++)
                {
                    btnArray[i, j].TextColor = Color.Blue;
                    btnArray[i, j].IsEnabled = false;
                    btnArray[i, j].BackgroundColor = Color.CadetBlue;
                    btnArray[i, j].Text = "" + matrizBase[i, j];
                }
            }
            Toast.MakeText(Android.App.Application.Context, "PERDISTE", ToastLength.Long).Show();
        }

        void abrirCero(int x, int y)
        {
            System.Diagnostics.Debug.WriteLine("abrircero: "+x + "," + y);
            if (y != _TAMANO_MATRIZ - 1)
            {
                descubrir(x, y + 1);

                if (x != _TAMANO_MATRIZ - 1)
                    descubrir(x + 1, y + 1);
                if (x != 0)
                    descubrir(x - 1, y + 1);
            }

            if (y != 0)
            {
                descubrir(x, y - 1);

                if (x != _TAMANO_MATRIZ - 1)
                    descubrir(x + 1, y - 1);
                if (x != 0)
                    descubrir(x - 1, y - 1);
            }

            if (x != _TAMANO_MATRIZ - 1)
                descubrir(x + 1, y);

            if (x != 0)
                descubrir(x - 1, y);
        
        }

        void automata(int entrada, int x, int y)
        {
            int[,] estados = {
                {1,2},
                {1,1},
                {2,3},
                {3,0}
            };

            matrizEdos[x, y] = estados[matrizEdos[x, y], entrada];
        }
    }
}
