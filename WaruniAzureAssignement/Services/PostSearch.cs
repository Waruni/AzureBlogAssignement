using System;
using System.Configuration;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace WaruniAzureAssignement.Services
{
    public class PostSearch
    {
        private static SearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        public static string errorMessage;

        static PostSearch()
        {
            try
            {
                string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
                string apiKey = ConfigurationManager.AppSettings["SearchServiceApiKey"];

                // Create an HTTP reference to the catalog index
                _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
                _indexClient = _searchClient.Indexes.GetClient("blogsearch");
            }
            catch (Exception e)
            {
                errorMessage = e.Message.ToString();
            }
        }

        public DocumentSearchResult Search(string searchText)
        {
            // Execute search based on query string
            try
            {
                SearchParameters sp = new SearchParameters() { SearchMode = SearchMode.All };
                return _indexClient.Documents.Search(searchText, sp);
            }
            catch (Exception ex)
            {
                throw new Exception("Error querying index: {0}\r\n" + ex.Message);
            }

            return null;
        }
    }
}