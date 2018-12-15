using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CloudPresentationDemoApp
{
    class Program
    {
        
        static void Main(string[] args)
        {
            #region Create Person

            //CreatePersonDocument().Wait();

            #endregion

            #region Query Person Information

            //RunSimpleQuery().Wait();

            #endregion

            #region Query Person Information (SQL)

            RunSQLQuery().Wait();

            #endregion

        }


        private static async Task CreatePersonDocument()
        {
            // Step 1. Connect to the Document Db Client.
            string EndpointUrl = "https://rahul.documents.azure.com:443/";
            string PrimaryKey = "26fvMrkzo6CFYMno2cS3dd05EmJBlF2djy2wg8SPtp36VglLM1wCzq0z74a6ugXBJKoAnvcBTSuvKOQwoIjk7w==";
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            
            // Step 2. Create the document. 
            string databaseName = "VoterDatabase";
            string collectionName = "VoterCollection";
            Person person = new Person()
            {
                Id = "231-231-231",
                FirstName = "Arnold",
                LastName = "Schwarzenegger",
                City = "Los Angeles",
                County = "Hollywood",
                State = "CA"
            };
            
            // Step 3. Add document to collection.
            try
            {
                await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), person);
                WriteToConsoleAndPromptToContinue("Created person of Id =  {0}", person.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task RunSimpleQuery()
        {
            // Step 1. Connect to the Document Db Client.
            string EndpointUrl = "https://rahul.documents.azure.com:443/";
            string PrimaryKey = "26fvMrkzo6CFYMno2cS3dd05EmJBlF2djy2wg8SPtp36VglLM1wCzq0z74a6ugXBJKoAnvcBTSuvKOQwoIjk7w==";
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            string databaseName = "VoterDatabase";
            string collectionName = "VoterCollection";

            // Step 2. Set up Query parameters.

            // Set some common query options
            var key = new PartitionKey("Los Angeles");
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, PartitionKey=key };

            // Here we find the person by last name.

            // Step 3. Define the query.
            IQueryable<Person> personQuery = client.CreateDocumentQuery<Person>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(f => f.LastName == "Schwarzenegger");

            // Step 4. Execute the query.
            Console.WriteLine("Running LINQ query...");
            List<Person> voters = personQuery.ToList();
            foreach (Person person in voters)
            {
                Console.WriteLine("\tRead {0} {1}", person.FirstName, person.City);
            }
            WriteToConsoleAndPromptToContinue("");

        }

        private static async Task RunSQLQuery()
        {
            // Step 1. Connect to the Document Db Client.
            string EndpointUrl = "https://rahul.documents.azure.com:443/";
            string PrimaryKey = "26fvMrkzo6CFYMno2cS3dd05EmJBlF2djy2wg8SPtp36VglLM1wCzq0z74a6ugXBJKoAnvcBTSuvKOQwoIjk7w==";
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            string databaseName = "VoterDatabase";
            string collectionName = "VoterCollection";

            // Step 2. Set up Query parameters.

            // Set some common query options
            var key = new PartitionKey("Los Angeles");
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, PartitionKey = key };


            // Step 3. Create the Query.
            // Create query via direct SQL.
            IQueryable<Person>  personQueryInSql = client.CreateDocumentQuery<Person>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    "SELECT * FROM c WHERE c.County = 'Hollywood'",
                    queryOptions);

            Console.WriteLine("Running direct SQL query...");

            // Step 4. Execute the query.
            foreach (Person person in personQueryInSql)
            {
                Console.WriteLine("\tRead {0} {1}", person.FirstName, person.LastName);
            }
            WriteToConsoleAndPromptToContinue("");
        }

        private static void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

    }
}
