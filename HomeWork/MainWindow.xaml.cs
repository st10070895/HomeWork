using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HomeWork;
using Newtonsoft.Json;

namespace HomeWork
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 
            Loaded += Window_Loaded; 
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fetch products and categories 
            var products = await GetProductsAsync();
            var categories = await GetCategoriesAsync();

            // Bind categories & Products
            comboBoxCategories.ItemsSource = categories;
            listViewProducts.ItemsSource = products;
        }

        // Method to fetch products from the FakeStoreAPI
        private async Task<List<Product>> GetProductsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://fakestoreapi.com/products"; // API endpoint for products
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync(); // Read the response as JSON
                    return JsonConvert.DeserializeObject<List<Product>>(json); // Deserialize JSON to a list of Product objects
                }
                return null;
            }
        }

        // Method to fetch categories from the FakeStoreAPI
        private async Task<List<string>> GetCategoriesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://fakestoreapi.com/products/categories"; // API endpoint for categories
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync(); // Read the response as JSON
                    return JsonConvert.DeserializeObject<List<string>>(json); // Deserialize JSON to a list of strings
                }
                return null;
            }
        }

        // Event handler for when the selected category in the ComboBox changes
        private async void comboBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedCategory = comboBoxCategories.SelectedItem as string;
            var products = await GetProductsAsync(); // Fetch all products
            var filteredProducts = products.Where(p => p.Category == selectedCategory).ToList(); // Filter products by category
            listViewProducts.ItemsSource = filteredProducts; // Bind filtered products to the ListView
        }

        // Event handler for the search button click
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchText = textBoxSearch.Text.ToLower(); // Get the search text and convert to lowercase
            var products = listViewProducts.ItemsSource as List<Product>; // Get the current list of products
            var searchedProducts = products.Where(p => p.Title.ToLower().Contains(searchText)).ToList(); // Filter products by title
            listViewProducts.ItemsSource = searchedProducts; // Bind searched products to the ListView
        }

        private bool isAscending = true; // Tracks the current sorting order (ascending or descending)

        // Event handler for the price sort button click
        private void buttonPriceSort_Click(object sender, RoutedEventArgs e)
        {
            var products = listViewProducts.ItemsSource as List<Product>;

            if (isAscending)
            {
                // Sort in ascending order
                var sortedProducts = products.OrderBy(p => p.Price).ToList();
                listViewProducts.ItemsSource = sortedProducts;
                buttonPriceSort.Content = "Sort by Price (Descending)";
            }
            else
            {
                // Sort in descending order
                var sortedProducts = products.OrderByDescending(p => p.Price).ToList();
                listViewProducts.ItemsSource = sortedProducts;
                buttonPriceSort.Content = "Sort by Price (Ascending)";
            }
            // Toggle the sorting order
            isAscending = !isAscending;
        }
    }
}