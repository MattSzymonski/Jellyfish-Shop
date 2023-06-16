import React, { useState, useEffect } from 'react';

const AdvancedList = () => {
    const [items, setItems] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalItemCount, setTotalItemCount] = useState(0);
    const [totalPageCount, setTotalPageCount] = useState(0);

    // Load on component creation
    useEffect(() => {
      fetchData(); 
    }, [currentPage]);
  
    const fetchData = async () => {
        
      try {
        const baseEndpoint = "http://localhost:4000/api/Jellyfish";
       
        const options = {
            method: "GET",
        }

        const params = new URLSearchParams({
            pagesize: 5,
            pagenumber: currentPage
        })

        const endpoint = baseEndpoint + "?" + params;
        console.log(endpoint); 
        const response = await fetch(endpoint, options);

        if (response.ok) {
            const data = await response.json(); // Whole response body (try printing it: "console.log(data);")
            
            if (data.status == "Success")
            {
                const { items, totalItemCount, totalPageCount } = data.data;
                console.log(data);
                setItems(items);
                setTotalItemCount(totalItemCount);
                setTotalPageCount(totalPageCount);
            }
       
        }
      } catch (error) {
        console.error('Error fetching data:', error);
      }
    };
  
    return (
      <div>
        <ul className="" >
          {items.map((item) => (
            <li key={item.id} className={"border-2 border-black bg-slate-600 p-2 m-2" } >
              {/* Render each item */}
              <p className="text-xl">{item.name}</p>
              <p>{item.behaviour}</p>
              <p>{item.price}</p>
            </li>
          ))}
        </ul>

        <div>
            {/* Render page buttons */}
            {Array.from({ length: totalPageCount }, (_, index) => (
            <button
                key={index}
                className={`m-2 mr-2 px-2 py-1 border ${ index === currentPage - 1 ? 'bg-blue-500 text-white' : 'bg-gray-200' }`}
                onClick={() => { setCurrentPage(index + 1); }}
            >
                {index + 1}
            </button>
            ))}
        </div>

      </div>
    );
  };
  
  export default AdvancedList;