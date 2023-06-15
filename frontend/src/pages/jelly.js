export default function Home() {
    return <p>Jelly Jelly Jelly</p>
}

export async function getStaticProps({ params }) {
    const request = await fetch(`https://jsonplaceholder.typicode.com/posts`);
    const data = await request.json();

    return {
        props: { car: data },
    }
}