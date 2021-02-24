package main

import "C"

func main() {
}

//export GenerateIssuerNonceB64
func GenerateIssuerNonceB64() *C.char {
	issuerNonceB64 := "GenerateIssuerNonceB64 has generated!"
	return C.CString(issuerNonceB64)
}

//export Issue
func Issue(issuerPkXml, issuerSkXml, issuerNonceB64, commitmentsJson string, attributes []string) *C.char {
	result := issuerPkXml + "|" + issuerSkXml + "|" + issuerNonceB64 + "|" + commitmentsJson + "|" + attributes[0]
	return C.CString(result)
}