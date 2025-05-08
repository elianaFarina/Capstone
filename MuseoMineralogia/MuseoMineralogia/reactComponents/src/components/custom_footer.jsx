import React from 'react';
import { MapPin, Phone, Mail, Clock, Instagram, Facebook, Twitter } from 'lucide-react';

const Footer = () => {
    const currentYear = new Date().getFullYear();

    return (
        <footer className="bg-dark text-white pt-5 pb-4">
            <div className="container">
                <div className="row">
                    {/* Informazioni di contatto */}
                    <div className="col-md-4 mb-4">
                        <h5 className="text-uppercase mb-4 font-weight-bold">Museo di Mineralogia</h5>
                        <div className="d-flex align-items-center mb-3">
                            <MapPin size={20} className="me-2" />
                            <p className="mb-0">Via dei Cristalli 123, Roma, Italia</p>
                        </div>
                        <div className="d-flex align-items-center mb-3">
                            <Phone size={20} className="me-2" />
                            <p className="mb-0">+39 123 456 7890</p>
                        </div>
                        <div className="d-flex align-items-center">
                            <Mail size={20} className="me-2" />
                            <p className="mb-0">info@museomineralogia.it</p>
                        </div>
                    </div>

                    {/* Orari */}
                    <div className="col-md-4 mb-4">
                        <h5 className="text-uppercase mb-4 font-weight-bold">Orari di apertura</h5>
                        <div className="d-flex align-items-center mb-3">
                            <Clock size={20} className="me-2" />
                            <div>
                                <p className="mb-0">Martedì - Venerdì: 9:00 - 17:00</p>
                                <p className="mb-0">Sabato - Domenica: 10:00 - 18:00</p>
                                <p className="mb-0">Lunedì: Chiuso</p>
                            </div>
                        </div>
                        <div className="mt-4">
                            <h6 className="mb-3">Biglietti</h6>
                            <p className="mb-1">Adulti: €10,00</p>
                            <p className="mb-1">Ridotto (studenti, over 65): €7,00</p>
                            <p className="mb-1">Bambini (sotto i 12 anni): Gratuito</p>
                        </div>
                    </div>

                    {/* Link utili e social */}
                    <div className="col-md-4 mb-4">
                        <h5 className="text-uppercase mb-4 font-weight-bold">Link utili</h5>
                        <ul className="list-unstyled">
                            <li className="mb-2"><a href="#" className="text-white text-decoration-none">Home</a></li>
                            <li className="mb-2"><a href="#" className="text-white text-decoration-none">Collezioni</a></li>
                            <li className="mb-2"><a href="#" className="text-white text-decoration-none">Eventi</a></li>
                            <li className="mb-2"><a href="#" className="text-white text-decoration-none">Visite guidate</a></li>
                            <li className="mb-2"><a href="#" className="text-white text-decoration-none">Contatti</a></li>
                        </ul>

                        <h5 className="text-uppercase mb-4 mt-4 font-weight-bold">Seguici</h5>
                        <div className="d-flex">
                            <a href="#" className="text-white me-3">
                                <Facebook size={24} />
                            </a>
                            <a href="#" className="text-white me-3">
                                <Instagram size={24} />
                            </a>
                            <a href="#" className="text-white">
                                <Twitter size={24} />
                            </a>
                        </div>
                    </div>
                </div>

                {/* Parte bassa del footer */}
                <hr className="my-4 bg-secondary" />
                <div className="row align-items-center">
                    <div className="col-md-8">
                        <p className="mb-md-0">
                            © {currentYear} Museo di Mineralogia - Tutti i diritti riservati
                        </p>
                    </div>
                    <div className="col-md-4">
                        <ul className="list-inline text-md-end mb-0">
                            <li className="list-inline-item">
                                <a href="#" className="text-white text-decoration-none small">Privacy Policy</a>
                            </li>
                            <li className="list-inline-item ms-3">
                                <a href="#" className="text-white text-decoration-none small">Cookie Policy</a>
                            </li>
                            <li className="list-inline-item ms-3">
                                <a href="#" className="text-white text-decoration-none small">Note Legali</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </footer>
    );
};

export default Footer;