﻿using Microsoft.AspNetCore.Builder.Extensions;
using System.Drawing;

namespace BLAZAM.Server.Shared.UI.Themes
{
    public class DarkTheme : ApplicationTheme
    {
        public DarkTheme()
        {
            _name = "Dark";
            _textLight = "#c5cbd3";
            _textDark = "#c9c6c6";
            _textSecondary = "#566874";
            _light = "#383b40";
            _dark = "#202226";
            _primary = "#a9a09f";
            _secondary = "#183042";
            _info = "#1b8f7e";
            _success = "#5fad00";
            _warning = "#ffc270";
            _danger = "#f60066";
            _body = _light;
            _muted = Color.DarkGray.ToHex();
          
        }
    }
}
