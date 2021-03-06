import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import { GET, DELETE } from "../../../api/apiServices";

import "../../../global.styles.css";
const BookManage = () => {
  const [bookData, setbookData] = useState(null);
  const [somethingChange, setsomethingChange] = useState(false);
  useEffect(() => {
    GET("books/all").then((res) => setbookData(res.data));
  }, [somethingChange]);

  const handleDelete = (id) => {
    if (window.confirm("Do your want Delete this item?")) {
      DELETE("books", id).then(
        () => {
          setsomethingChange(!somethingChange);
        },
        (err) => console.log(err)
      );
    }
  };

  console.log(bookData);
  return (
    <div>
      <h1>Category Update</h1>

      <table id="customers">
        <thead>
          <tr>
            <th>Id</th>
            <th>Title</th>
            <th>Category</th>
            <th>Author</th>
            <th>Available Quantity</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {bookData ? (
            bookData.map((elm) => (
              <tr key={elm.id}>
                <td>{elm.id}</td>
                <td>{elm.title}</td>
                <td>{elm.category.name}</td>
                <td>{elm.author.name}</td>
                <td>{elm.quantity}</td>
                <td>
                  <button onClick={() => handleDelete(elm.id)}>Delete</button>
                  <br />
                  <button>
                    <Link
                      style={{ color: "white" }}
                      to={`/bookupdate/${elm.id}`}
                    >
                      Update
                    </Link>
                  </button>
                </td>
              </tr>
            ))
          ) : (
            <h1>Loading...</h1>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default BookManage;
