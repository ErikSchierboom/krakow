const path = require("path");

module.exports = function(_, argv) {
  const mode = argv.mode || "development";
  const isProduction = mode === "production";

  return {
    mode: mode,
    devtool: isProduction ? false : "eval-source-map",
    entry: "./src/Krakow.Website.fsproj",
    output: {
      filename: "bundle.js",
      path: path.join(__dirname, "./public")
    },
    devServer: {
      contentBase: "./public",
      port: 8080
    },
    module: {
      rules: [
        {
          test: /\.fs(x|proj)?$/,
          use: "fable-loader"
        }
      ]
    }
  };
};
