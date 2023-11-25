import React, { useState } from "react";
import axios from "axios";
import { useNavigate, Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function CreateItemDepartment() {
  const [id, setId] = useState("");
  const [categoryName, setCategoryName] = useState("");
  const [itemDepartmentParentId, setItemDepartmentParentId] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [parent, setParent] = useState(true);

  const navigate = useNavigate();

  const getToken = () => {
    return localStorage.getItem("token");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    const effectiveItemDepartmentParentId = parent
      ? "0"
      : itemDepartmentParentId;

    try {
      let axiosConfig = {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${getToken()}`, // Include the token from local storage
        },
        method: "POST",
        url: "https://localhost:7240/api/itemdepartment",
        data: {
          id: id,
          categoryName: categoryName,
          itemDepartmentParentId: effectiveItemDepartmentParentId,
        },
      };

      let response = await axios(axiosConfig);
      console.log(response.data);
      setId("");
      setCategoryName("");
      setItemDepartmentParentId("");
      navigate("/item-department");
    } catch (error) {
      console.error(error);
      setError(JSON.stringify(error, Object.getOwnPropertyNames(error)));
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppLayout>
      <h2 className="page-title">Create a new Item Department</h2>
      <div className="card mt-3">
        <div className="card-body">
          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">Category Name</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the category name"
                value={categoryName}
                onChange={(e) => setCategoryName(e.target.value)}
              />
            </div>
            <div className="mb-3">
              <label className="form-check form-switch">
                <input
                  className="form-check-input"
                  type="checkbox"
                  checked={parent}
                  onChange={() => setParent(!parent)}
                />
                <span className="form-check-label">Use as a parent?</span>
              </label>
            </div>
            <div className="mb-3">
              <label className="form-label">Item Department Parent ID</label>
              <input
                type="text"
                className="form-control"
                placeholder="Enter the item department parent ID"
                value={itemDepartmentParentId}
                onChange={(e) => setItemDepartmentParentId(e.target.value)}
                disabled={parent}
              />
            </div>
            <div className="mb-3">
              <input type="submit" value="Save" className="btn btn-primary" />
            </div>
            <div className="mb-3">
              <Link to="/item-department" className="btn btn-primary">
                Cancel
              </Link>
            </div>
          </form>
          {error && (
            <div className="alert alert-danger" role="alert">
              {error}
            </div>
          )}
        </div>
      </div>
    </AppLayout>
  );
}

export default CreateItemDepartment;
