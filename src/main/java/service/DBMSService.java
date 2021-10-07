package service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import repository.XMLRepository;
import domain.Database;

@Service
public class DBMSService {

    @Autowired
    private XMLRepository repository;

    public Database createDatabase(String name){
        return this.repository.createDatabase(name);
    }
}
