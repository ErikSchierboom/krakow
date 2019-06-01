const path = require("path");

module.exports = (env, options) => {
  const isProduction = options.mode === "production";

  return {
    mode: "development",
    entry: {
      bunlde: ["./src/App.fsproj", "./src/styles/index.scss"]
    },
    output: {
      path: path.join(__dirname, "./public"),
      filename: "[name].js"
    },
    devServer: {
      contentBase: "./public",
      port: 8888
    },
    module: {
      rules: [
        {
          test: /\.fs(x|proj)?$/,
          use: "fable-loader"
        },
        {
          test: /\.s?css$/,
          use: ["style-loader", "css-loader", "sass-loader"]
        }
      ]
    }
  };
};
