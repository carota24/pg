#r "Microsoft.Azure.Documents.Client"
#r "Newtonsoft.Json"
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using System.Net;
 
public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    
    string cosmosdbEndpoint= System.Environment.GetEnvironmentVariable("cosmosdbEndpoint", EnvironmentVariableTarget.Process); 
    string cosmosdbAccountKey= System.Environment.GetEnvironmentVariable("cosmosdbAccountKey", EnvironmentVariableTarget.Process); 
    string collectionID="usageCollection";
    string DatabaseId="PG_Usage";
    
    
    DocumentClient client;
    dynamic adata = await req.Content.ReadAsAsync<object>();
    string vstsAccount = adata?.vstsAccount;
    string TemplateName = adata?.TemplateName;
    string ProjectName = adata?.ProjectName;
    string AzuresubscriptionID = adata?.AzuresubscriptionID;
    string AzuresubscriptionName = adata?.AzuresubscriptionName;
    string endUser = adata?.endUser;
    string executionResult=adata?.executionResult;

    client = new DocumentClient(new Uri(cosmosdbEndpoint), cosmosdbAccountKey);
    await client
         .CreateDatabaseIfNotExistsAsync(
         new Database { Id = DatabaseId });
 
    DocumentCollection myCollection = new DocumentCollection();


    myCollection.Id = collectionID;
            myCollection.IndexingPolicy = 
           new IndexingPolicy(new RangeIndex(DataType.String) 
                                  { Precision = -1 });
    
    myCollection.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
        var res=await client.CreateDocumentCollectionIfNotExistsAsync(
            UriFactory.CreateDatabaseUri(DatabaseId),
            myCollection);

    object item = new {
         vstsAccount =  vstsAccount,
         TemplateName = TemplateName,
         localDate= DateTime.Now,
         ProjectName=ProjectName,
         AzuresubscriptionID=AzuresubscriptionID,
         AzuresubscriptionName=AzuresubscriptionName,
         endUser=endUser,
         executionResult=executionResult
    };

    await client
                .CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, collectionID),item);
    
    var myObj = new {result = "OK"};
    var json2Return = JsonConvert.SerializeObject(myObj);


    return vstsAccount == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name in the request body")
        : req.CreateResponse(HttpStatusCode.OK,json2Return);
}
