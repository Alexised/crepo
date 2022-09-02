import { Modal, Row, Col, Form, Select, Input, Button, Upload } from "antd";
import { React, useRef, useCallback, useEffect, useState } from "react";
import axios from "axios";
import Config from "../../helpers/Config";
import Cookies from "js-cookie";
import { UploadOutlined } from "@ant-design/icons";
import Webcam from "react-webcam";

const { Option } = Select;
const videoConstraints = {
  width: 1280,
  height: 720,
  facingMode: "user",
};

export const AddDocument = ({
  isModalAddDocVisible,
  setModalAddDocVisible,
  loading,
  setLoading,
  userId,
}) => {
  const API_URL = "https://localhost:7252/api";
  const [cohorts, setCohorts] = useState([]);
  const authorization = Cookies.get("access-token");
  const [isMobileDevice, setIsMobileDevice] = useState(false);
  const webcamRef = useRef("");
  const [show, setShow] = useState(false);
  const [isModalAddTakePhoto, setIsModalAddTakePhoto] = useState(false);
  const [image, setImage] = useState(undefined);
  const capture = useCallback(() => {
    const imageSrc = webcamRef.current.getScreenshot();
    setImage(imageSrc);
    setState({
      ...state,
      dataFile: {
        ...state.dataFile,
        base64: imageSrc,
        name: "capture",
        error: false,
      },
    });
  }, [webcamRef]);
  const [state, setState] = useState({
    dataFile: {
      base64: "",
      name: "",
      error: false,
    },
  });

  const getBase64 = (file, cb) => {
    const reader = new FileReader();
    if (!file) return;
    reader.onload = function () {
      cb(reader.result);
    };
    reader.readAsDataURL(file);

    reader.onerror = function (error) {
      setState({
        ...state,
        dataFile: {
          ...state.dataFile,
          error: true,
        },
      });
    };
  };

  const handleChangeFile = (info) => {
    if (!state.dataFile.error && info.file.status !== "removed") {
      getBase64(info.file, (imageUrl) =>
        setState({
          ...state,
          dataFile: {
            ...state.dataFile,
            base64: imageUrl,
            name: info.file.name,
            error: false,
          },
        })
      );
    } else {
      setState({
        ...state,
        dataFile: {
          ...state.dataFile,
          base64: "",
          name: "",
        },
      });
    }
  };

  const handleShow = () => {
    setIsModalAddTakePhoto(true);
  };

  const [form] = Form.useForm();
  form.setFieldsValue({
    dniType: "CC",
    regime: "Subsidiado",
    cohort: 1,
  });

  const onFinish = (values) => {
    const doc = {
      DniType: values.dniType,
      Dni: values.dni,
      UserId: userId,
      DocumentFile: state.dataFile.base64,
      Regime: values.regime,
      CohortId: values.cohort,
    };

    axios
      .post(`${Config.API_URL}/Document/create-document`, doc, {
        //axios.post(`${API_URL}/Document/create-document`, doc, {
        headers: { Authorization: "Bearer " + authorization },
      })
      .then((res) => {})
      .catch((err) => {})
      .finally(() => {
        form.resetFields();
        setLoading(false);
        setModalAddDocVisible(false);
      });
  };

  const handleCancel = () => {
    form.resetFields();
    setModalAddDocVisible(false);
    setShow(false);
  };
  const handleCancelphoto = () => {
    setIsModalAddTakePhoto(false);
  };

  useEffect(() => {}, [isModalAddDocVisible, userId]);
  useEffect(() => {
    let details = navigator.userAgent;
    /* Creating a regular expression 
        containing some mobile devices keywords 
        to search it in details string*/
    let regexp = /android|iphone|kindle|ipad/i;
    setIsMobileDevice(regexp.test(details)); //if (edit) {
  }, [isModalAddDocVisible]);
  useEffect(() => {
    axios
      .get(`${Config.API_URL}/document/cohorts`, {
        headers: { Authorization: "Bearer " + authorization },
      })
      .then(({ data }) => {
        setCohorts([...data]);
      });
    //}
  }, [authorization, form]);

  return (
    <>
      <Modal
        style={{ right: 55 }}
        title={"Agregar Documento"}
        visible={isModalAddDocVisible}
        onCancel={handleCancel}
        footer={[
          <Button key="back" onClick={handleCancelphoto}>
            Cancelar
          </Button>,
          <Button
            key="submit"
            className="btn-primary"
            type="primary"
            onClick={() => form.submit()}
            loading={loading}
          >
            {!loading ? "Guardar" : "Cargando..."}
          </Button>,
        ]}
      >
        <Form layout="vertical" widht="100%" form={form} onFinish={onFinish}>
          <Row>
            <Col span={24}>
              <Row>
                <Col span={24}>
                  <Form.Item label="Régimen" style={{ marginBottom: "-10px" }}>
                    <Input.Group>
                      <Form.Item
                        name="regime"
                        rules={[
                          {
                            required: true,
                            message: "Por favor seleccione un régimen!",
                          },
                        ]}
                        style={{ width: "100%" }}
                      >
                        <Select
                          placeholder="Régimen"
                          style={{ width: "100%" }}
                          name="regime"
                          defaultValue={"Subsidiado"}
                        >
                          <Option value="Subsidiado">Subsidiado</Option>
                          <Option value="Contributivo">Contributivo</Option>
                        </Select>
                      </Form.Item>
                    </Input.Group>
                  </Form.Item>
                </Col>
              </Row>
              <Row>
                <Col span={24}>
                  <Form.Item label="Cohorte" style={{ marginBottom: "-10px" }}>
                    <Input.Group>
                      <Form.Item
                        name="cohort"
                        rules={[
                          {
                            required: true,
                            message: "Por favor seleccione una cohorte!",
                          },
                        ]}
                        style={{ width: "100%" }}
                      >
                        <Select
                          placeholder="Cohorte"
                          style={{ width: "100%" }}
                          name="cohort"
                          defaultValue={1}
                        >
                          {cohorts.map((cohort) => (
                            <Option
                              key={cohort.cohortId}
                              value={cohort.cohortId}
                            >
                              {cohort.name}
                            </Option>
                          ))}
                        </Select>
                      </Form.Item>
                    </Input.Group>
                  </Form.Item>
                </Col>
              </Row>
              <Row>
                <Col span={24}>
                  <Form.Item
                    label="Tipo documento"
                    style={{ marginBottom: "-10px" }}
                  >
                    <Input.Group>
                      <Form.Item
                        name="dniType"
                        rules={[
                          {
                            required: true,
                            message:
                              "Por favor seleccione un tipo de documento!",
                          },
                        ]}
                        style={{ width: "100%" }}
                      >
                        <Select
                          placeholder="Tipo Documento"
                          style={{ width: "100%" }}
                          name="dniType"
                          defaultValue={"CC"}
                        >
                          <Option value="CC">CC</Option>
                          <Option value="CE">CE</Option>
                          <Option value="MS">MS</Option>
                          <Option value="PE">PE</Option>
                          <Option value="PT">PT</Option>
                          <Option value="RC">RC</Option>
                          <Option value="SC">SC</Option>
                          <Option value="TI">TI</Option>
                        </Select>
                      </Form.Item>
                    </Input.Group>
                  </Form.Item>
                </Col>
              </Row>
              <Form.Item
                label="Número documento"
                style={{ marginBottom: "0px" }}
              >
                <Input.Group>
                  <Form.Item
                    align="left"
                    name="dni"
                    type="number"
                    rules={[
                      {
                        required: true,
                        message: "Por favor ingresa el documento de identidad!",
                      },
                    ]}
                    style={{ width: "100%" }}
                  >
                    <Input
                      style={{ width: "100%" }}
                      type="number"
                      placeholder="71375294"
                      name="dni"
                    />
                  </Form.Item>
                </Input.Group>
              </Form.Item>
            </Col>
          </Row>
          <Row>
            <Form.Item label="Documento">
              <Input.Group>
                {!isMobileDevice && (
                  <Form.Item
                    align="left"
                    name="voucher"
                    rules={[
                      {
                        required: true,
                        message: "Por favor ingresa un archivo!",
                      },
                    ]}
                  >
                    <Upload
                      accept="application/pdf,image/png,image/jpg,image/jpeg"
                      name="file"
                      valuePropName="fileList"
                      multiple={false}
                      maxCount={1}
                      beforeUpload={(file) => {
                        const isAllowed =
                          file.type === "application/pdf" ||
                          file.type === "image/png" ||
                          file.type === "image/jpg" ||
                          file.type === "image/jpeg";
                        if (!isAllowed) {
                          alert(
                            "Solo se admiten documentos PDF e imagenes JPG y PNG."
                          );
                          return Upload.LIST_IGNORE;
                        }
                        return false;
                      }}
                      onChange={handleChangeFile}
                    >
                      <Button icon={<UploadOutlined />}>
                        Click para cargar
                      </Button>
                    </Upload>
                  </Form.Item>
                )}
              </Input.Group>
            </Form.Item>
            <div>
              {isMobileDevice && (
                <button onClick={handleShow}> tomar foto</button>
              )}
            </div>
          </Row>
          <Row>
            {image && (
              <img
                style={{
                  textAlign: "center",
                  zindex: 8,
                  right: 0,
                  height: "350px",
                  width: "350px",
                  objectFit: "fill",
                }}
                src={image}
                alt="test-ilustartion"
              />
            )}
          </Row>
        </Form>
      </Modal>
      <Modal
        style={{ right: 55 }}
        title={"Tomar Foto"}
        visible={isModalAddTakePhoto}
        onCancel={handleCancelphoto}
        footer={[
          <Button key="back" onClick={handleCancelphoto}>
            Cancelar
          </Button>,
        ]}
      >
        <Form layout="vertical" widht="100%" form={form}>
          <Row>
            <Col span={24}></Col>
          </Row>
          <Row>
            {!image ? (
              <>
                <Webcam
                  style={{
                    textAlign: "center",
                    zindex: 8,
                    right: 0,
                    height: "550px",
                    width: "350px",
                    objectFit: "fill",
                  }}
                  forceScreenshotSourceSize="true"
                  audio={false}
                  ref={webcamRef}
                  screenshotFormat="image/jpeg"
                  videoConstraints={videoConstraints}
                />
              </>
            ) : (
              <img
                style={{
                  textAlign: "center",
                  zindex: 8,
                  right: 0,
                  height: "550px",
                  width: "350px",
                  objectFit: "fill",
                }}
                src={image}
                alt="test-ilustartion"
              />
            )}
            <button onClick={() => (!image ? capture() : setImage(undefined))}>
              {!image ? "tomar foto" : "tomar otra foto"}
            </button>
          </Row>
        </Form>
      </Modal>
    </>
  );
};
