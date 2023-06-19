using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace App2
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Pokemon> pokemons;

        public MainPage()
        {
            InitializeComponent();

            pokemons = new ObservableCollection<Pokemon>();
            pokemonListView.ItemsSource = pokemons;

            LoadPokemonList();
        }

        private async void LoadPokemonList()
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://pokeapi.co/api/v2/pokemon?limit=800");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var pokemonList = JsonConvert.DeserializeObject<PokemonList>(json);

                foreach (var pokemonInfo in pokemonList.Results)
                {
                    var pokemonResponse = await httpClient.GetAsync(pokemonInfo.Url);
                    pokemonResponse.EnsureSuccessStatusCode();
                    var pokemonJson = await pokemonResponse.Content.ReadAsStringAsync();
                    var pokemon = JsonConvert.DeserializeObject<Pokemon>(pokemonJson);
                    pokemons.Add(pokemon);
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error de la solicitud HTTP
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void OnPokemonSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedPokemon = (Pokemon)e.SelectedItem;
            pokemonListView.SelectedItem = null;

            await DisplayAlert(selectedPokemon.Name, $"ID: {selectedPokemon.Id}\n\nWeight: {selectedPokemon.Weight}\n\nHeight: {selectedPokemon.Height}\n\n", "OK");
        }
    }

    public class PokemonList
    {
        [JsonProperty("results")]
        public List<PokemonInfo> Results { get; set; }
    }

    public class PokemonInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Pokemon
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        
    

        [JsonProperty("weight")]
        public int Weight { get; set; }

        

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("sprites")]
        public Sprites Sprites { get; set; }

        public string SpriteUrl => Sprites?.FrontDefault;
    }

    public class Sprites
    {
        [JsonProperty("front_default")]
        public string FrontDefault { get; set; }
    }
}
