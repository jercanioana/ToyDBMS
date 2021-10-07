package repository;

import domain.Database;
import org.springframework.stereotype.Repository;

@Repository
public class XMLRepository {
    private String fileName;

    public XMLRepository(String fileName) {
        this.fileName = fileName;
    }

    public Database createDatabase(String name){
        return new Database(name);
    }
}
