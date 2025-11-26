using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace LapsAI.Shared.Services
{
    public class StorageService
    {
        private readonly IJSRuntime _jS;



        public StorageService(IJSRuntime jS)
        {
            _jS = jS;

        }

        public ValueTask Set(string key, string value)
        {
            return _jS.InvokeVoidAsync("localStorage.setItem", key, value);

        }

        public ValueTask<string> Get(string key)
        {
            return _jS.InvokeAsync<string>("localStorage.getItem", key);
        }
    }
}
