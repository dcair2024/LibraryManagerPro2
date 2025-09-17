from flask import Flask, request, jsonify
import random
import hashlib

app = Flask(__name__)

# Lista de imagens para usar como exemplo
IMAGENS_EXEMPLO = [
    "https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300&h=400&fit=crop", 
    "https://images.unsplash.com/photo-1481627834876-b7833e8f5570?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1529651737248-dad5e20a9f5f?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1512820790803-83ca734da794?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1535905557558-afc4877cdf3f?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1519904981063-b0cf448d479e?w=300&h=400&fit=crop",
    "https://images.unsplash.com/photo-1506880018603-83d5b814b5a6?w=300&h=400&fit=crop"
]

@app.route('/')
def home():
    return jsonify({
        "message": "API de Geração de Capas está funcionando!",
        "endpoints": {
            "gerar_capa": "POST /gerar-capa",
            "exemplo": {
                "titulo": "Nome do Livro",
                "descricao": "Descrição do livro"
            }
        }
    })

@app.route('/gerar-capa', methods=['POST'])
def gerar_capa():
    try:
        # Pega os dados JSON da requisição
        data = request.get_json()
        
        if not data:
            return jsonify({"error": "JSON não fornecido"}), 400
            
        titulo = data.get('titulo', '')
        descricao = data.get('descricao', '')
        
        if not titulo:
            return jsonify({"error": "Título é obrigatório"}), 400
        
        # Gera uma imagem baseada no hash do título (será sempre a mesma para o mesmo título)
        hash_titulo = hashlib.md5(titulo.encode()).hexdigest()
        indice = int(hash_titulo, 16) % len(IMAGENS_EXEMPLO)
        
        url_imagem = IMAGENS_EXEMPLO[indice]
        
        # Log para debug
        print(f"📚 Gerando capa para: '{titulo}' -> {url_imagem}")
        
        # Retorna a resposta no formato que o C# espera
        return jsonify({
            "url": url_imagem,
            "titulo": titulo,
            "descricao": descricao,
            "status": "success"
        })
        
    except Exception as e:
        print(f"❌ Erro ao gerar capa: {str(e)}")
        return jsonify({
            "error": f"Erro interno: {str(e)}",
            "status": "error"
        }), 500

@app.route('/test')
def test():
    """Endpoint para testar se a API está funcionando"""
    return jsonify({
        "message": "API Flask funcionando!",
        "total_imagens": len(IMAGENS_EXEMPLO)
    })

if __name__ == '__main__':
    print("🚀 Iniciando API de Geração de Capas...")
    print("📍 Endpoints disponíveis:")
    print("   GET  / - Informações da API")
    print("   POST /gerar-capa - Gerar capa de livro")
    print("   GET  /test - Teste simples")
    
    app.run(debug=True, port=5001, host='0.0.0.0')