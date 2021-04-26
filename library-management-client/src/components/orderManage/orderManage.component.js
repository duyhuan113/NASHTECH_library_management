import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

import { GET, PUT } from "../../api/apiServices";

const OrderManage = () => {
  const [orderData, setOrderData] = useState(null);
  const [somethingChange, setSomethingChange] = useState(false);
  useEffect(() => {
    GET("orders/all").then((res) => setOrderData(res.data));
  }, [somethingChange]);

  const handleUpdate = (id, status) => {
    PUT(`orders`, id, { status: status }).then(() =>
      setSomethingChange(!somethingChange)
    );
  };

  const setStatus = (id, status) => {
    if (status === "confirm") {
      return <span>confirm</span>;
    } else if (status === "reject") {
      return <span>reject</span>;
    } else {
      return (
        <>
          <button onClick={() => handleUpdate(id, "confirm")}>confirm</button>
          <button onClick={() => handleUpdate(id, "reject")}>reject</button>
        </>
      );
    }
  };

  return (
    <div>
      <table id="customers">
        <thead>
          <tr>
            <th>Id</th>
            <th>Quantity</th>
            <th>Status</th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {orderData ? (
            orderData.map((elm) => (
              <tr key={elm.id}>
                <td>{elm.id}</td>
                <td>{elm.orderDetails.length}</td>
                <td>{setStatus(elm.id, elm.status)}</td>
                <td>
                   <Link to={`/orderdetail/${elm.id}`}>Detail</Link>
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

export default OrderManage;