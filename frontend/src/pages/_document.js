import { Html, Head, Main, NextScript } from 'next/document'

export default function Document() {
  return (
    <Html lang="en">
        <Head>
          <meta charset="UTF-8" />
          {/* Favicon */}
          <link rel="icon" href="/favicons/favicon.ico" />
          <link rel="apple-touch-icon" sizes="180x180" href="/favicons/apple-touch-icon.png" />
          <link rel="icon" type="image/png" sizes="32x32" href="/favicons/favicon-32x32.png" />
          <link rel="icon" type="image/png" sizes="16x16" href="/favicons/favicon-16x16.png" />
          <link rel="manifest" href="/favicons/site.webmanifest" />
          <link rel="icon" type="image/png" sizes="192x192" href="/favicons/android-chrome-192x192.png" />
          <link rel="icon" type="image/png" sizes="512x512" href="/favicons/android-chrome-512x512.png" />
          {/* Browser address bar */}
          <meta name="msapplication-TileColor" content="#ffffff" />
          <meta name="msapplication-TileImage" content="/favicons/mstile-150x150.png" />
          {/* Fonts */}
          <link href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap" rel="stylesheet" />
          <link href="https://fonts.googleapis.com/css?family=Roboto+Condensed:400,700&display=swap" rel="stylesheet" />
          <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />
          <link href="https://code.ionicframework.com/ionicons/2.0.1/css/ionicons.min.css" rel="stylesheet" />
          <link href="https://fonts.googleapis.com/css2?family=Inter&display=swap" rel="stylesheet" />
          <link href="https://fonts.googleapis.com/css2?family=Libre+Franklin:wght@900&display=swap" rel="stylesheet" />
      </Head>
      {/* Load Tailwind Elements */}
      <body>
        <Main />
        <NextScript />
      </body>
      <script type="text/javascript" src="../node_modules/tw-elements/dist/js/tw-elements.umd.min.js"></script>
    </Html>
  )
}
