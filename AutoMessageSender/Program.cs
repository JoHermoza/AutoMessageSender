using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

class Program
{
    static async Task Main()
    {
        string connectionString = "Endpoint=sb://jh-test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=DHUAdzLRDfJdRFE1md8NofQ6C6798byBD+ASbM+h2ro=;";
        string topicName = "test-local-iterable-topic";
        string subscriptionName = "test-local-iterable-sub";

        // Create a ServiceBusClient
        await using var client = new ServiceBusClient(connectionString);

        // Create a ServiceBusSender for the topic
        ServiceBusSender sender = client.CreateSender(topicName);

        try
        {
            var length = 400;

            for (int i = 1; i <= length; i++)
            {

                var jsonString = "{\"email\": \"debounce_050723_v3_test_" + i + "@hotmail.com\",\"website\": \"tzinsurance.com\",\"sapoLeadId\": \"f39b3d7d-f2d6-4c21-823a-764cc4b0dfd2\",\"cuySessionId\": " +
                                "\"262027451\",\"caseStatus\": \"New\",\"policyEffectiveDate\": \"2023-06-01\"," +
                                "\"formattedPolicyEffectiveDate\": \"06-01-2023\",\"planName\": " +
                                "\"Anthem MediBlue Dual Access (PPO D-SNP)\",\"carrier\": \"Anthem\"," +
                                "\"productType\": \"Medicare Advantage\",\"productLine\": \"Health\"," +
                                "\"cuyCampaign\": \"SMA\",\"firstName\": \"jaime\",\"lastName\": " +
                                "\"heredia\",\"state\": \"CA\",\"dateOfBirth\": \"1942-09-28\"," +
                                "\"dateOfBirthDay\": \"28\",\"dateOfBirthMonth\": \"9\",\"dateOfBirthYear\": " +
                                "\"1942\",\"nestedCaseDetails\": [  {    \"sapoLeadId\": \"f39b3d7d-f2d6-4c21-823a-764cc4b0dfd2\",    " +
                                "\"cuySessionId\": \"262027451\",    \"caseStatus\": \"New\",    \"policyEffectiveDate\": \"2023-06-01\",    " +
                                "\"planName\": \"Anthem MediBlue Dual Access (PPO D-SNP)\",    \"carrier\": \"Anthem\",    " +
                                "\"productType\": \"Medicare Advantage\",    \"productLine\": \"Health\",    \"cuyCampaign\": \"SMA\"  }]}";


                JsonDocument jsonDocument = JsonDocument.Parse(jsonString);


                ServiceBusMessage message = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonString));
                message.ApplicationProperties["SubscriptionName"] = subscriptionName;
                await sender.SendMessageAsync(message);

                Console.WriteLine("Message sent successfully.");

            }
        }
        finally
        {
            // Close the sender and client
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}
