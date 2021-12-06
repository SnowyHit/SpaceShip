using System;
using RestSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading;

public class APIManager : MonoBehaviour
{
    public string URL = "https://graphapi.devargedor.com/graphql";
    public string AppId = "5";

    public delegate void APIResult(object param);

    public void Login(string userName, string password, APIResult onSuccess, APIResult onFailure)
    {
        var client = new RestClient(URL);
        client.Timeout = -1;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        //request.AddParameter("application/json", "{\"query\":\"mutation login ($user: LoginUserInput) {\\n    login (user: $user) {\\n        info {\\n            status\\n            message\\n        }\\n        token\\n        user {\\n            id\\n            username\\n            email\\n            createdAt\\n            x_chain_public_key\\n            x_chain_private_key\\n            c_chain_public_key\\n            c_chain_private_key\\n            c_chain_private_key_hex\\n        }\\n    }\\n}\",\"variables\":{\"user\":{\"username\":\"" + userName.Trim() + "\",\"password\":\"" + password.Trim() + "\"}}}", ParameterType.RequestBody);
        request.AddParameter("application/json", "{\"query\":\"query getPlayer ($player: ReadPlayerInput) {\\n    getPlayer (player: $player) {\\n        info {\\n            status\\n            message\\n        }\\n        player {\\n            id\\n            gamePlayerID\\n            appId\\n            username\\n            password\\n            x_chain_public_key\\n            x_chain_private_key\\n            c_chain_public_key\\n            c_chain_private_key\\n            c_chain_private_key_hex\\n        }\\n    }\\n}\",\"variables\":{\"player\":{\"gamePlayerID\":\"" + userName.Trim() + "\"}}}", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);

        try
        {
            JObject o = JObject.Parse(response.Content);

            if (o["data"]["getPlayer"]["info"]["status"].ToString() == "true")
            {
                onSuccess?.Invoke(o);
            }
            else
            {
                onFailure?.Invoke(null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error on login: " + ex.Message);
            onFailure?.Invoke(null);
        }
    }

    public void Register(string email, string userName, string password, APIResult onSuccess, APIResult onFailure)
    {
        var client = new RestClient(URL);
        client.Timeout = -1;

        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", "{\"query\":\"mutation createPlayer ($player: CreatePlayerInput) {\\n    createPlayer (player: $player) {\\n        info {\\n            status\\n            message\\n        }\\n        player {\\n            id\\n            gamePlayerID\\n            appId\\n            username\\n            password\\n            x_chain_public_key\\n            x_chain_private_key\\n            c_chain_public_key\\n            c_chain_private_key\\n            c_chain_private_key_hex\\n        }\\n    }\\n}\",\"variables\":{\"player\":{\"appId\":" + AppId + ",\"gamePlayerID\":\"" + userName.Trim() + "\"}}}", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);

        try
        {
            JObject o = JObject.Parse(response.Content);

            if (o["data"]["createPlayer"]["info"]["status"].ToString() == "true")
            {
                onSuccess?.Invoke(o);
            }
            else
            {
                onFailure?.Invoke(o["data"]["createPlayer"]["info"]["message"].ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error on login: " + ex.Message);
            onFailure?.Invoke(ex.Message);
        }
    }

    public void GetPlayerTokens(string userName, APIResult onSuccess)
    {
        var client = new RestClient("https://graphapi.devargedor.com/graphql");
        client.Timeout = -1;
        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", "{\"query\":\"query getPlayerTokenCount ($playerx: PlayerOwnersInput) {\\n    getPlayerTokenCount (playerx: $playerx) {\\n        count\\n    }\\n}\",\"variables\":{\"playerx\":{\"gamePlayerID\":\"" + userName.Trim() + "\",\"appId\":" + AppId + "}}}", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);

        try
        {
            JObject o = JObject.Parse(response.Content);

            float result = float.Parse(o["data"]["getPlayerTokenCount"]["count"].ToString());

            onSuccess?.Invoke(result);
        }
        catch (Exception ex)
        {
            Debug.Log("Error on getPlayerTokens: " + ex.Message);
            onSuccess?.Invoke(-1);
        }
    }

    public void SendTokensToPlayer(string userName, float tokenAmount, APIResult onComplete)
    {
        new Thread(() =>
        {
            var client = new RestClient("https://graphapi.devargedor.com/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"mutation sendToken ($sendToken: SendToken) {\\n    sendToken (sendToken: $sendToken) {\\n        info {\\n            status\\n            message\\n        }\\n    }\\n}\",\"variables\":{\"sendToken\":{\"gamePlayerID\":\"" + userName + "\",\"amount\":\"" + tokenAmount + "\"}}}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            try
            {
                JObject o = JObject.Parse(response.Content);

                if (o["data"]["sendToken"]["info"]["status"].ToString() == "true")
                {
                    onComplete?.Invoke("OK");
                }
                else
                {
                    onComplete?.Invoke("MSG:" + o["data"]["sendToken"]["info"]["message"].ToString());
                }
            }
            catch (Exception ex)
            {
                onComplete?.Invoke("ERROR:" + ex.Message);
            }
        }).Start();
    }

    public void GetNFTs(string userName, APIResult onComplete)
    {
        var client = new RestClient("https://graphapi.devargedor.com/graphql");
        client.Timeout = -1;
        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", "{\"query\":\"query getPlayerOwners ($playerOwners: PlayerOwnersInput) {\\n    getPlayerOwners (playerOwners: $playerOwners) {\\n        info {\\n            status\\n            message\\n        }\\n        playerOwners {\\n            id\\n            name\\n            description\\n            image_path\\n            metadata_url\\n            metadata_json\\n            token_id\\n            collectionId\\n            type\\n            count\\n        }\\n    }\\n}\",\"variables\":{\"playerOwners\":{\"gamePlayerID\":\"" + userName + "\",\"appId\":" + AppId + "}}}", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);

        try
        {
            onComplete?.Invoke(response.Content);
        }
        catch (Exception ex)
        {
            onComplete?.Invoke("ERROR:" + ex.Message);
        }
    }
    public void Mint(string userName, int collectionId, APIResult onComplete)
    {
        var client = new RestClient("https://graphapi.devargedor.com/graphql");
        client.Timeout = -1;
        var request = new RestRequest(Method.POST);
        request.AddHeader("Content-Type", "application/json");
        request.AddParameter("application/json", "{\"query\":\"mutation mintNFT ($mintNFT: MintNFTInput) {\\n    mintNFT (mintNFT: $mintNFT) {\\n        gamePlayerID\\n        collectionId\\n        transactions\\n    }\\n}\",\"variables\":{\"mintNFT\":{\"gamePlayerID\":\"" + userName.Trim() + "\",\"collectionId\":" + collectionId + "}}}", ParameterType.RequestBody);
        IRestResponse response = client.Execute(request);

        try
        {
            onComplete?.Invoke(response.Content);
        }
        catch (Exception ex)
        {
            onComplete?.Invoke("ERROR:" + ex.Message);
        }
    }
}
