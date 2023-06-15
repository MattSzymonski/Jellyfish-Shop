// Import icons as components
import { AiFillHeart } from 'react-icons/ai';

// Import state hook
import { useState } from 'react';

import { useRouter } from 'next/router'

// Head allows to easily add HTML code to <head> tag of the page
import Head from 'next/head'

// "jellyfish" object is passed from getStaticProps automatically
// We can retrieve data from it and use it in HTML code here
export default function Jellyfish({ jellyfish }) {
    const router = useRouter()
    const { name } = router.query

    // count is a getter while setCount is a setter
    const [likesCount, setLikesCount] = useState(0);

    return (<>
        <Head>
            <title>Jellyfish - { name }</title>
        </Head>
        <div className='jellyfish-card'>
            <h1>Jellyfish: { name }</h1>
            <p>Color: { jellyfish.color }</p>
            <p>Description: { jellyfish.description }</p>
            <p>Likes: { likesCount }</p>
            <img src = { jellyfish.image } />
           
            <AiFillHeart size="32px" onClick={ () => setLikesCount(likesCount + 1)} />
        </div>
    </>)
}

// This function is automatically called by Next.js before the component is rendered
// "params" object contains the route parameters 
// (eg. if page is [name].js, params will be { name: <SOMETHING> })
export async function getStaticProps({ params }) {

    // Fetch data from given URL
    const request = await fetch(`http://localhost:3000/static/jellyfish_onomicon_data/${params.name}.json`);
  
    // Parses request result JSON to JavaScript object
    const data = await request.json();
  
    // This is sent to the component. In this case jellyfish object containig fetched data
    return {
      props: { jellyfish: data }    
    }
}

// This function has to return "paths" object with all possible paths for this dynamic route
export async function getStaticPaths() {

    // Hardcoded values! 
    // Better approach would be to create another JSON file to store this data and fetch it
    const paths = [{ params: { name: "MoonGlow" } }, { params: { name: "PinkPuff" } }]

    // Return paths with some additional options
    return {
        paths, 
        fallback: false
    }
}
