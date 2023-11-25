import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function ShowBrand() {
  const [brands, setBrands] = useState([]);

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  const deleteBrand = (id) => {
    fetch(`https://localhost:7240/api/brand/${id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${getToken()}`, // Use the token from local storage
      },
    }).then(() => {
      setBrands(brands.filter((brand) => brand.id !== id));
    });
  };

  useEffect(() => {
    fetch("https://localhost:7240/api/brand", {
      headers: {
        Authorization: `Bearer ${getToken()}`, // Use the token from local storage
      },
    })
      .then((res) => res.json())
      .then((data) => setBrands(data));
  }, []);

  return (
    <AppLayout>
      <h2 className="page-title">Brand</h2>
      <div className="card mt-3">
        <div className="card-body">
          <div className="col-12">
            <div className="mb-3">
              <Link to="/brand/create" className="btn btn-primary">
                Create new
              </Link>
            </div>
            <div className="card">
              <div className="table-responsive">
                <table className="table table-vcenter card-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Brand Name</th>
                      <th>Action</th>
                      <th className="w-1"></th>
                    </tr>
                  </thead>
                  <tbody>
                    {brands.map((brand) => (
                      <tr key={brand.id}>
                        <td>{brand.id}</td>
                        <td>{brand.name}</td>
                        <td>
                          <Link
                            to={`/brand/edit/${brand.id}`}
                            className="btn btn-primary"
                          >
                            Edit
                          </Link>
                          <button
                            className="btn btn-danger mx-2"
                            onClick={() => deleteBrand(brand.id)}
                          >
                            Delete
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </AppLayout>
  );
}

export default ShowBrand;
