import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import AppLayout from "../../layouts/AppLayout";

function ShowItemSupplierTransaction() {
  const [itemsuppliertransactions, setItemSupplierTransactions] = useState([]);
  const [items, setItems] = useState([]);
  const [suppliers, setSuppliers] = useState([]);
  const [itemsuppliers, setItemSuppliers] = useState([]);

  // Function to get the token from local storage
  const getToken = () => {
    return localStorage.getItem("token");
  };

  const deleteItemSupplierTransaction = (id) => {
    fetch(`https://localhost:7240/api/itemsupplier_transaction/${id}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${getToken()}`, // Include the token from local storage
      },
    }).then(() => {
      setItemSupplierTransactions(
        itemsuppliertransactions.filter((tx) => tx.id !== id)
      );
    });
  };

  useEffect(() => {
    const fetchResource = async (url, setter) => {
      try {
        const response = await fetch(url, {
          headers: {
            Authorization: `Bearer ${getToken()}`, // Include the token from local storage
          },
        });
        const data = await response.json();
        setter(data);
      } catch (error) {
        console.error("Error fetching data:", error);
      }
    };

    fetchResource("https://localhost:7240/api/item", setItems);
    fetchResource("https://localhost:7240/api/supplier", setSuppliers);
    fetchResource("https://localhost:7240/api/itemsupplier", setItemSuppliers);
  }, []);

  useEffect(() => {
    const fetchTransactionsAndEnrich = async () => {
      try {
        const response = await fetch(
          "https://localhost:7240/api/itemsupplier_transaction",
          {
            headers: {
              Authorization: `Bearer ${getToken()}`, // Include the token from local storage
            },
          }
        );
        const transactionsData = await response.json();

        if (items.length && suppliers.length && itemsuppliers.length) {
          const enrichedData = transactionsData.map((tx) => {
            const itemSupplier = itemsuppliers.find(
              (is) => is.id.toString() === tx.itemSupplierId.toString()
            );
            if (!itemSupplier) {
              return {
                ...tx,
                itemName: "Unknown Item",
                supplierName: "Unknown Supplier",
              };
            }
            const item = items.find(
              (itm) => itm.id.toString() === itemSupplier.itemId.toString()
            );
            const supplier = suppliers.find(
              (sup) => sup.id.toString() === itemSupplier.supplierId.toString()
            );

            return {
              ...tx,
              itemName: item ? item.itemName : "Unknown Item",
              supplierName: supplier
                ? supplier.supplierName
                : "Unknown Supplier",
            };
          });

          setItemSupplierTransactions(enrichedData);
        }
      } catch (error) {
        console.error("Error fetching transactions:", error);
      }
    };

    fetchTransactionsAndEnrich();
  }, [items, suppliers, itemsuppliers]);

  return (
    <AppLayout>
      <h2 className="page-title">Item Supplier Transaction</h2>
      <div className="card mt-3">
        <div className="card-body">
          <div className="col-12">
            <div className="mb-3">
              <Link
                to="/item-supplier-transaction/create"
                className="btn btn-primary"
              >
                Create new
              </Link>
            </div>
            <div className="card">
              <div className="table-responsive">
                <table className="table table-vcenter card-table">
                  <thead>
                    <tr>
                      <th>ID</th>
                      <th>Item Name</th>
                      <th>Supplier Name</th>
                      <th>Type</th>
                      <th>Date</th>
                      <th>Quantity</th>
                      <th>Notes</th>
                      <th>Action</th>
                      <th className="w-1"></th>
                    </tr>
                  </thead>
                  <tbody>
                    {itemsuppliertransactions.map((itemsuppliertransaction) => (
                      <tr key={itemsuppliertransaction.id}>
                        <td>{itemsuppliertransaction.id}</td>
                        <td>{itemsuppliertransaction.itemName}</td>
                        <td>{itemsuppliertransaction.supplierName}</td>
                        <td>{itemsuppliertransaction.transactionType}</td>
                        <td>{itemsuppliertransaction.transactionDate}</td>
                        <td>{itemsuppliertransaction.quantity}</td>
                        <td>{itemsuppliertransaction.notes}</td>
                        <td>
                          <Link
                            to={`/item-supplier-transaction/edit/${itemsuppliertransaction.id}`}
                            className="btn btn-primary"
                          >
                            Edit
                          </Link>
                          <button
                            className="btn btn-danger mx-2"
                            onClick={() =>
                              deleteItemSupplierTransaction(
                                itemsuppliertransaction.id
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

export default ShowItemSupplierTransaction;
