﻿using GraphQL.Types;
using GraphQLDotNet.Contracts;

namespace GraphQLDotNet.API.GraphTypes
{
    public class WeatherSummaryType : ObjectGraphType<WeatherSummary>
    {
        public WeatherSummaryType()
        {
            // TODO: Better field descriptions
            Field(x => x.Id).Description("The Id of the forecast location.");
            Field(x => x.Location).Description("Location of the measurement.");
            Field(x => x.Date).Description("The Date of the forecast.").Type(new DateTimeGraphType());
            Field(x => x.Temperature).Description("The Temperature in C.");
            Field(x => x.OpenWeatherIcon).Description("Icon id of the weather.");
            Field(x => x.Timezone).Description("The Timezone");
            Field(x => x.Clouds).Description("The cloud coverage in percentage");
        }
    }
}
