// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.WeatherMockApi;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly WeatherRecognizer _luisRecognizer;
        private readonly WeatherMockApi _weatherApi;
        protected readonly ILogger Logger;

        private readonly WeatherInfo _weatherInfoMock;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(
            WeatherRecognizer luisRecognizer,
            WeatherMockApi weatherApi,
            ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _weatherApi = weatherApi;
            Logger = logger;

            _weatherInfoMock = _weatherApi.GetMockWeatherInfo();

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with?\nSay something like \"What is the weather like in Paris?\"";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // LUIS is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(CancelAndHelpDialog), new WeatherInfo(), cancellationToken);
            }

            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<Weather>(stepContext.Context, cancellationToken);
            switch (luisResult.TopIntent().intent)
            {
                case Weather.Intent.GetWeather:
                    var weatherInfo = _weatherApi.GetMockWeatherInfo();
                    var getWeatherMessageText = $"It's {_weatherInfoMock.Description}, with a temerature of {weatherInfo.Temperature} degrees.";
                    var getWeatherMessage = MessageFactory.Text(getWeatherMessageText, getWeatherMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getWeatherMessage, cancellationToken);
                    break;
                case Weather.Intent.GetTemperature:
                    var getTemperatureMessageText = $"There are {_weatherInfoMock.Temperature} degrees outside.";
                    var getTemperatureMessage = MessageFactory.Text(getTemperatureMessageText, getTemperatureMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getTemperatureMessage, cancellationToken);
                    break;
                case Weather.Intent.GetWindSpeed:
                    var getWindSpeedMessageText = $"The wind speed is {_weatherInfoMock.WindSpeed} m/s.";
                    var getWindSpeedMessage = MessageFactory.Text(getWindSpeedMessageText, getWindSpeedMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(getWindSpeedMessage, cancellationToken);
                    break;
                case Weather.Intent.IsItRaining:

                    var raining = _weatherInfoMock.Description == "raining";

                    string isItRainingMessageText;

                    if (raining)
                    {
                        isItRainingMessageText = "It is raining.";
                    } else
                    {
                        isItRainingMessageText = $"It's not raining. Actually, it's {_weatherInfoMock.Description}.";
                    }

                    var isItRainingMessage = MessageFactory.Text(isItRainingMessageText, isItRainingMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(isItRainingMessage, cancellationToken);
                    break;
                case Weather.Intent.IsitSnowing:

                    var snowing = _weatherInfoMock.Description == "snowing";

                    string isItSnowingMessageText;

                    if (snowing)
                    {
                        isItSnowingMessageText = "It is snowing.";
                    }
                    else
                    {
                        isItSnowingMessageText = $"It's not snowing. Actually, it's {_weatherInfoMock.Description}.";
                    }

                    var isItSnowingMessage = MessageFactory.Text(isItSnowingMessageText, isItSnowingMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(isItSnowingMessage, cancellationToken);
                    break;
                case Weather.Intent.IsItSunny:

                    var sunny = _weatherInfoMock.Description == "sunny";

                    string isItSunnyMessageText;

                    if (sunny)
                    {
                        isItSunnyMessageText = "It is sunny.";
                    }
                    else
                    {
                        isItSunnyMessageText = $"It's not sunny. Actually, it's {_weatherInfoMock.Description}.";
                    }

                    var isItSunnyMessage = MessageFactory.Text(isItSunnyMessageText, isItSunnyMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(isItSunnyMessage, cancellationToken);
                    break;
                default:
                    // Catch all for unhandled intents
                    var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {luisResult.TopIntent().intent})";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.ReplaceDialogAsync(InitialDialogId, cancellationToken);
        }
    }
}
