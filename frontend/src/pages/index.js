import Head from 'next/head'
import Image from 'next/image'
import { Inter } from 'next/font/google'
import AdvancedList from '/src/components/AdvancedList';

const inter = Inter({ subsets: ['latin'] })

export default function Home() {
  return (
  <>
     <Head>
        <title>Jellyfish Shop</title>
        <meta name="description" content="Jellyfish Shop Main Page" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
      </Head>
      <main>
        <div className="mx-auto container">
          <div className="bg-gray-500">
            <h1 className="text-center text-3xl">Jellyfish List</h1>
            <AdvancedList />
          </div>
        </div>
        
      </main>
  </>
  )
}
