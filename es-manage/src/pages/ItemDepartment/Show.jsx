import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom"; // Import Link from react-router-dom
import AppLayout from "../../layouts/AppLayout";

function ShowItemDepartment() {
  const [departments, setDepartments] = useState([]);

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  const deleteDepartment = (id, categoryName) => {
    fetch(`https://localhost:7240/api/itemdepartment/${id}/${categoryName}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${getToken()}`, // Use the token from local storage
      },
    }).then(() => {
      setDepartments(departments.filter((department) => department.id !== id));
    });
  };

  useEffect(() => {
    // Fetch Item Departments
    fetch("https://localhost:7240/api/itemdepartment", {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${getToken()}`, // Use the token from local storage
      },
    })
      .then((res) => res.json())
      .then((data) => setDepartments(data));
  }, []);

  return (
    <AppLayout>
      <h2 className="page-title">Item Department</h2>
      <div className="card mt-3">
        <div className="card-body">
          <div className="col-12">
            <div className="mb-3">
              <Link to="/item-department/create" className="btn btn-primary">
                Create new
              </Link>
            </div>
            <div className="card">
              <div className="table-responsive">
                <table className="table table-vcenter card-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Category Name</th>
                      <th>Parent ID</th>
                      <th>Action</th>
                      <th className="w-1"></th>
                    </tr>
                  </thead>
                  <tbody>
                    {departments.map((department) => (
                      <tr key={department.id}>
                        <td>{department.id}</td>
                        <td>{department.categoryName}</td>
                        <td>{department.itemDepartmentParentId}</td>
                        <td>
                          <Link
                            to={`/item-department/edit/${department.id}/${department.categoryName}`}
                            className="btn btn-primary"
                          >
                            Edit
                          </Link>
                          <button
                            className="btn btn-danger mx-2"
                            onClick={() =>
                              deleteDepartment(
                                department.id,
                                department.categoryName
                              )
                            }
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

export default ShowItemDepartment;
