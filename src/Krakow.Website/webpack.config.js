const path = require("path");

module.exports = {
  mode: "development",
  entry: "./src/App.fsproj",
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
      }
    ]
  }
};
